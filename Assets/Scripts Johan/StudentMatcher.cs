using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class StudentMatcher : MonoBehaviour
{
    [System.Serializable]
    public class StudentProfile
    {
        public string studentId;
        public string studentName;
        public MatchingProfile profile;
        public float compatibilityScore;
    }
    
    // Calculate compatibility between two profiles (0-100)
    public static float CalculateCompatibility(MatchingProfile profile1, MatchingProfile profile2)
    {
        // Calculate difference for each attribute (lower difference = better match)
        float morningDiff = Mathf.Abs(profile1.morningPerson - profile2.morningPerson);
        float groupDiff = Mathf.Abs(profile1.groupStudy - profile2.groupStudy);
        float seriousnessDiff = Mathf.Abs(profile1.seriousness - profile2.seriousness);
        float talkativeDiff = Mathf.Abs(profile1.talkative - profile2.talkative);
        float visualDiff = Mathf.Abs(profile1.visual - profile2.visual);
        float practicalDiff = Mathf.Abs(profile1.practical - profile2.practical);
        float theoreticalDiff = Mathf.Abs(profile1.theoretical - profile2.theoretical);
        
        // Weight each attribute (you can adjust these)
        float weightedDiff = 
            morningDiff * 1.2f +      // Study time is important
            groupDiff * 1.5f +         // Group preference is very important
            seriousnessDiff * 1.3f +   // Goal alignment is important
            talkativeDiff * 0.8f +     // Communication style moderately important
            visualDiff * 0.6f +        // Learning style less critical
            practicalDiff * 0.6f +
            theoreticalDiff * 0.6f;
        
        // Total possible difference (if everything was max different)
        float maxPossibleDiff = 10 * (1.2f + 1.5f + 1.3f + 0.8f + 0.6f + 0.6f + 0.6f);
        
        // Convert to compatibility score (0-100)
        float compatibility = (1 - (weightedDiff / maxPossibleDiff)) * 100;
        
        return Mathf.Clamp(compatibility, 0, 100);
    }
    
    // Find best matches for a student from a list of other students
    public static List<StudentProfile> FindBestMatches(MatchingProfile userProfile, List<StudentProfile> potentialMatches, int topN = 5)
    {
        // Calculate compatibility with each student
        foreach (StudentProfile student in potentialMatches)
        {
            student.compatibilityScore = CalculateCompatibility(userProfile, student.profile);
        }
        
        // Sort by compatibility (highest first)
        List<StudentProfile> sortedMatches = potentialMatches
            .OrderByDescending(s => s.compatibilityScore)
            .Take(topN)
            .ToList();
        
        return sortedMatches;
    }
    
    // Get a text description of the compatibility
    public static string GetCompatibilityDescription(float score)
    {
        if (score >= 85) return "Excellent Match! 🔥";
        if (score >= 70) return "Great Match!";
        if (score >= 55) return "Good Match";
        if (score >= 40) return "Moderate Match";
        return "Low Match";
    }
    
    // Get specific reasons why two profiles match well
    public static List<string> GetMatchingReasons(MatchingProfile profile1, MatchingProfile profile2)
    {
        List<string> reasons = new List<string>();
        
        // Check each attribute
        if (Mathf.Abs(profile1.morningPerson - profile2.morningPerson) <= 2)
        {
            if (profile1.morningPerson >= 7)
                reasons.Add("Both prefer studying in the morning");
            else if (profile1.morningPerson <= 3)
                reasons.Add("Both are night owls");
            else
                reasons.Add("Similar study time preferences");
        }
        
        if (Mathf.Abs(profile1.groupStudy - profile2.groupStudy) <= 2)
        {
            if (profile1.groupStudy >= 7)
                reasons.Add("Both love group study sessions");
            else if (profile1.groupStudy <= 3)
                reasons.Add("Both prefer studying solo");
            else
                reasons.Add("Balanced approach to group/solo study");
        }
        
        if (Mathf.Abs(profile1.seriousness - profile2.seriousness) <= 2)
        {
            if (profile1.seriousness >= 7)
                reasons.Add("Both are highly focused students");
            else
                reasons.Add("Similar intensity in study goals");
        }
        
        if (Mathf.Abs(profile1.talkative - profile2.talkative) <= 2)
        {
            reasons.Add("Compatible communication styles");
        }
        
        return reasons;
    }
     public async Task<string> FindBestGroupForStudent(string userId, MatchingProfile userProfile, int maxGroupSize = 4)
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;
        var groupsSnap = await db.Child("groups").GetValueAsync();

        if (!groupsSnap.Exists)
        {
            Debug.Log("No groups exist yet.");
            return null; // caller should create a new group
        }

        string bestGroupId = null;
        float bestGroupScore = -1f;

        foreach (var group in groupsSnap.Children)
        {
            int size = group.Child("size").Exists ? Convert.ToInt32(group.Child("size").Value) : 0;
            if (size >= maxGroupSize) continue; // skip full groups

            // gather member ids
            var usersNode = group.Child("users");
            List<string> memberIds = new List<string>();
            foreach (var member in usersNode.Children)
                memberIds.Add(member.Key);

            // if a group is empty (rare), skip
            if (memberIds.Count == 0) continue;

            // load member profiles in parallel
            var tasks = memberIds.Select(id => db.Child("users").Child(id).Child("profile").GetValueAsync()).ToArray();
            await Task.WhenAll(tasks);

            List<StudentProfile> members = new List<StudentProfile>();
            for (int i = 0; i < tasks.Length; i++)
            {
                var snap = tasks[i].Result;
                if (snap.Exists)
                {
                    try
                    {
                        // Expect profile stored as JSON under /users/{uid}/profile
                        MatchingProfile mp = JsonUtility.FromJson<MatchingProfile>(snap.GetRawJsonValue());
                        members.Add(new StudentProfile { studentId = memberIds[i], profile = mp });
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"Failed to parse profile for {memberIds[i]}: {e.Message}");
                    }
                }
            }

            if (members.Count == 0) continue;

            // compute average compatibility vs members
            float total = 0f;
            foreach (var m in members)
                total += CalculateCompatibility(userProfile, m.profile);

            float avgCompatibility = total / members.Count;

            // choose the group with the highest avgCompatibility
            if (avgCompatibility > bestGroupScore)
            {
                bestGroupScore = avgCompatibility;
                bestGroupId = group.Key;
            }
        }

        // if bestGroupScore is very low, you might prefer to return null to force new group creation.
        // e.g. if (bestGroupScore < 30) return null;
        return bestGroupId;
    }
}

