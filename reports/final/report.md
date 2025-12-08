# Studygram DPM 5 Report
Gamify Labs: Alua Kaliazhdarova, Johan Ronnquist, Sche In Baek, Olesia Bilyk
## Quality Arguments
University students study alone instead of meeting new people to study together. Our app gamifies the process and matches students to work on the coursework together, making the process of studying less isolated and thus more efficient. Our approach emphasizes long-term teamwork through **matching** within class based on learning type of students, smoothened one-click **group study sessions**, and shared resources that all group members contribute to by collaborating on **shared flashcard sets**, which results in imprved learning habits and efficiency through collaboration.


<img src="images/Welcome Page.png" alt="Welcome Page" width="200"/><img src="images/Quiz.png" alt="Quiz" width="200"/>



<img src="images/Group Session.png" alt="Group Session" width="200"/><img src="images/Session Reward.png" alt="Session Reward" width="200"/>

<img src="images/New Card.png" alt="New Card" width="200"/>
<img src="images/Study Mode.png" alt="Study Mode" width="200"/>

<img src="images/Community Garden.png" alt="Community Garden" width="200"/>

## Deployment Summary


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

### Individual Reflections
#### Alua


#### Sche In


#### Johan
1. **Implementation Contribution**\
My contributions focused on the visual environment and the core system mechanics. Visually, I used Generative AI to create cohesive assets (sprites, icons, and backgrounds) and built the UI for the "Room," "Garden," and "Quiz" scenes to ensure a consistent aesthetic. Technically, I implemented the "Study Buddy" matching logic using Firebase, which required coding the profiling quiz and connecting the results to the database to group users based on compatibility. I also built the main navigation system that seamlessly connects these scenes and the specific logic for the Quiz interactions.
1. **Teamwork Reflection**\
Our team generally collaborated well, but we faced challenges due to varying experience levels with full-stack development. Initially, we adopted a "siloed" approach, developing front-end and back-end separately and hoping they would integrate without issues. This led to some friction when connecting the components. We overcame this by shifting from working in isolation to actively helping and teaching each other during the integration process. The key lesson I learned is that successful teamwork isn't just about dividing tasks; it requires constant communication and a willingness to learn from one another to bridge skill gaps and ensure the system works as a whole.
1. Through this project, I gained significant experience with the Unity UI system, specifically learning how to manage dynamic layouts and navigation stacks. Crucially, I learned how to integrate Firebase with Unity, understanding the technical pipeline required to connect a game engine to a cloud database for real-time data synchronization. Additionally, I refined my workflow for using Generative AI as a production tool, learning how to prompt and edit AI-generated assets to fit a specific game style rapidly.



#### Olesia
