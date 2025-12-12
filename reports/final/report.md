# Studygram DPM 5 Report
Gamify Labs: Alua Kaliazhdarova, Johan Ronnquist, Sche In Baek, Olesia Bilyk
## Quality Arguments
Our app is designed to gamify the process of social studying and match students to work on the coursework together, which was achieved with "Learning Type" quiz the students are asked to complete at the beginning of their journey with Studygram. \
<img src="images/quiz.png" width="200"/><img src="images/group_alloc.png" width="200"/>

After being matched to a group, users gain access to Social Garden that upgrades when all members study on the app, reflected both in group progress bar and garden design. \
<img src="images/lousy_garden.png" width="200"/><img src="images/cool_garden.png" width="200"/>

Users can see each other's avatars in the garden and thus find out if there are available buddies to start a study session with. After navigating to group study screen from the social menu, users can start a timed session together. They are not required to actually meet, and what app aims to achieve is the feeling of presence of others (similarly to how people study better at cafes and other coworking places). \
<img src="images/session_menu.png" width="200"/><img src="images/session.png" width="200"/>

Another social feature is flashcard sets that are shared across the entire class. Users can add cards they created to existing sets or create new ones. Having both Multiple Choice Question and Term/Definition options, flashcards are the simplest form of shared learning resources that could be useful for most of classes and is easy to contribute to.
<img src="images/card_database.png" width="200"/><img src="images/study_mode.png" width="200"/>

Both creating flashcards and partaking in group study sessions earns users coins that allow them to buy sets created by others, thus requiring everyone in the class to contribute in order to get benefits from using the app. \
<img src="images/reward.png" width="200"/><img src="images/card_creat.png" width="200"/>

We chose pixel art for UI design along with other gamifying aspects (pet avatars, flashcard colour options) in order to make the process of studying feel more relaxing (which was acheived according to user testing).



## Deployment Summary
Our deployment included 5 KAIST undergraduates testing the app over a week-long period (it included people of three genders and from three countries; there was no point in diversifying the age since our target group is college students). They were assigned to two groups, with a member of our team in each for assistance. Instead of introducing our goal and core tasks to the users, we said that it's an app for social studying to see how intuitive the features and rules are.

<img src="images/intuitive.png" width="400"/><img src="images/visual.png" width="400"/>

The users found the app intuitive to use, but suggested that "introducing tooltips once the game starts might be useful". All of the users enjoyed the gamification design, both ones who appreciated the social aspect of Studygram (suggesting future improvement of "could be enhanced further whether friends could send each other support words") and ones who did not ("Why do i need to buy flashcards😭😭😭 I just wanna study" whereas the flashcard purchase was implemented so that students need to first get money by contributing to the group with session participation or card creation in order to get access to the shared resources), which suggests that studying-related apps in general could benefit from gamification.

<img src="images/awkwardness.png" width="400"/><img src="images/would_use.png" width="400"/>

Users agreed that Studygram reduces the initiation awkwardness (poll options ranging from 1 - "Not at all" to 5 - "Very much so"), and the majority would like to use the app for their real coursework. The experience could be improved if the users "were able to communicate with each other somehow", could "have a companion website to upload questions on to", or could preview flashcards before purchase ("can't really see them before buying"), among other things. The general consensus of the users was that the app does achieve its goal and is "really needed among super introverted KAIST students".


## Discussion
When we designed Studygram, we thought deeply about several social computing concepts to solve the problem of student isolation. We wanted to understand what actually motivates people to work together.

### Incentives for Participation

We used a mix of rewards to keep users engaged. We realized that just giving people points, which is an extrinsic reward, often leads to burnout. It creates short-term activity but not long-term commitment. That is why we designed the Social Garden. It taps into intrinsic motivation and social pressure in a positive way. Since the garden only grows when the whole team studies, it changes the motivation. Users start participating because they want to help their group thrive rather than just to get a high score for themselves.
### Supporting Social Interaction
A key lesson we learned is that being social does not always require active talking. Many existing tools fail because they force shy students to chat or turn on their video. We focused on the idea of passive presence. By allowing avatars to move around a shared virtual room, we recreated the feeling of a quiet library. You feel the comfort of being around others without the stress of forcing a conversation. This lowers the barrier for introverted students to feel connected. It proves that just being present is a powerful form of social interaction.
### Matching and Homophily
Our matching system addresses the challenge of forming a good team. Randomly assigned groups often fail because the members have different expectations. We used the concept of homophily, which is the idea that people get along better with those who are similar to them. By grouping students based on their Learning Types, such as Visual Learner or Grinder, we ensure that teams start with compatible study styles. This similarity reduces the friction when the group first meets and helps them trust each other faster.
### Privacy and Ethics
Finally, we treated privacy as a bridge rather than a barrier. We intentionally do not require real names at the start to reduce the fear of judgment. This anonymity allows users to interact based solely on their study habits without social anxiety. However, we recognize that the ultimate goal is connection. By using the app as a safe buffer to build trust first, users can eventually choose to take their study group into the real world. The system lowers the initial risk of approaching a stranger, making that optional transition to offline meeting much safer and more comfortable.




### URLs
You can download our renewed .apk prototype through the following [link](https://drive.google.com/file/d/14_wDQM7ZANa9IOcMF33LQf6yoTR1DyJP/). Please refer to the previous report for the emulator usage if needed.\
[Our Git Repository](https://github.com/sbaek293/Studygram/)

