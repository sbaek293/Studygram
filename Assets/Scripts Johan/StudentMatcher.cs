using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
}
