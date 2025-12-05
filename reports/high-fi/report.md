# Studygram DPM 4 Report
Gamify Labs: Alua Kaliazhdarova, Johan Ronnquist, Sche In Baek, Olesia Bilyk
## Project Summary
University students study alone instead of meeting new people to study together. Our app gamifies the process and matches students to work on the coursework together, making the process of studying less isolated and thus more efficient. Our approach emphasizes long-term teamwork through **matching** within class based on learning type of students, smoothened one-click **group study sessions**, and shared resources that all group members contribute to by collaborating on **shared flashcard sets**, which results in imprved learning habits and efficiency through collaboration.

### Instruction
**First Steps**
1. Enter your username at Welcome Screen. (We do not have passwords since the app is used for social studying and the worst malicious users could do would be study while pretending to be someone else and copy studying resources of others.)
1. You will be moved to the "Room Scene". From there you can access "Social Garden" and your "Profile" stats. You can also see your coins (earned by studying together) and progress (which is group progress so it will remain 0 unless you join a group).
1. When entering the Garden for the first time, you will be redirected to the Study Type Quiz.
1. The quiz consists of 8 Multiple Choice Questions. After finishing it you will be invited to join a group of at most three other people based on the compatibility of your studying preferences.
1. Finishing the quiz will give you a virtual avatar based on your type. From now on, it will be present on your Room and Garden screens. In the social Garden, you will also be able to see others' avatars to feel their presence. You can move around with WASD keys to spot your teammates if they are also currently online.

<img src="images/Welcome Page.png" alt="Welcome Page" width="200"/><img src="images/Quiz.png" alt="Quiz" width="200"/>

**Group Session**
1. From the Garden, you can click "Home" to go back to the Room. You can also click "Menu" (three horizontal lines) which is the menu of your social studying tfrom which you can navigate to group session window or flashcards window.
1. In the Group Session window, you will be able to see all currently active study sessions that you can join with one click. If there are none, the list will be empty.
1. You can press "+" to start your own session that will immediately be displayed on other people's lists of active sessions.
1. You will need at least one other person to join for the "Play" button to be enabled (otherwise it wouldn't be very social).
1. When you (the host) press "Play", the timer starts. The number of coins each of the participants will accumulate is based on the duration of the session.
1. You can pause and resume the session.
1. Once you end the session, it will end for everyone. On each participant's screen a pop-up will appear stating the experiece points and coins learned.
1. You can exit the Group Session window and go back to the social Garden by pressing the arrow in the top left of the screen.

<img src="images/Group Session.png" alt="Group Session" width="200"/><img src="images/Session Reward.png" alt="Session Reward" width="200"/>

**Flashcard Creation**
1. Navigate to Flashcards main screen from Garden scene.
1. Flashcards main screen is the Card Menu. From there you can both study with cards and create new ones.
1. You can click "Refresh" button (refresh icon) to make sure the sets were updated if others modified them.
1. In order to create a new card, press "+" at the bottom of your screen.
1. In the Card Creation menu, by default youw ill be creating a Multiple Choice Question card, but you can click the dropdown menu and switch to the Defition card.
1. For the Definition type, you need to enter a term and its definition (front and back of the card respectively). 
1. For MCQ, you need to enter the question and two to four answer choices. You also need to mark one of them as correct.
1. In both cases, the app will not let you save the card if any fields are left empty.
1. Pressing "Change Color" button will change the colour of the flashcard you're working on to the one displayed on the button. 
1. Once you finish, press "Save".
1. You will be asked which set to add your card to. Since the sets are shared within the group, you can add it to a set you do not own.
1. You can also chose to add it to a New Set. In that case you will be asked to enter the set name which should be different from already existing one.
1. Once you complete the card creation, you will be awarded ten coins and redirected back to the Card Menu screen.

<img src="images/New Card.png" alt="New Card" width="200"/>

**Flashcard Study Mode**
1. The card sets you purchased/created are shown in their colour and the ones you do not have access to are displayed in grey. You can click on them, see the price and proceed with the purchase or cancel. It is the only way to spend earned coins and this design choice is to prevent people from accessing others' resources without contributing. Contribution is just being an active member of the community, so both flashcard creation and group study session participation are contributions in that sense.
1. Click on a set to practice with it. You will have "Left Arrow" and "Right Arrow" buttons to navigate between cards of the set if there is more than one. 
1. "Definition" cards flip as the usual flashcards. For "MCQ", you can click on answer choices to find out if they are correct. Once you are done practicing, press "Back" (arrow) button at the top of your screen to return to the Card Menu.
 
<img src="images/Study Mode.png" alt="Study Mode" width="200"/>

**Other Features**
1. At this point you might be wondering what were the level and progress bar for. It accounts for the experience users gained together through the study sessions. Once the team levels up, their community Garden design upgrades. This is to reward users for their shared progress. As mentioned above, the coin system is to ensure everybody's contribution to the shared progress.
1. We have automatic log in from the device thatw as used for the app before. It is done in order to lower the number of menial tasks to do before getting to the studying itself.
1. Gamified design serves the purpose of studying appearing more relaxing, closer to hanging out with friends rather than "wasting one's youth away reading textbooks late at night". We hope this design choice prompts no association with the pressure and stressful deadlines which official university apps do.

<img src="images/Community Garden.png" alt="Community Garden" width="200"/>

## Prototype
### URL of the Prototype
You can download our .apk prototype through the following [link](https://drive.google.com/drive/folders/1zstCLRff8KRO9V8c2D1-REH6bdeF2kmM). If you're not using ndroid, you will need to install an Android emulator to run it.
Example of running it on a Macbook:
- Download [Android Studio](https://developer.android.com/studio)
- Open Settings -> Language & Frameworks -> Android SDK -> SDK Tools
- Click on and install Android SDK Command-line Tools and Android Emulator
- Exit Settings and create an empty project (Empty Activity one)
- Go to View -> Tool Windows -> Device Manager
- Creat Virtual Device
- Choose Pixel 7 or a similar device with ARM 64 System Image
- Wait for the SDK Component Installation
- Run the phone simulation
- Drag and drop Studygram.apk on the simulated phone's screen
- Exit to the Main Screen of the phone
- Click on Studygram (the app with the Unity icon)
- Enjoy!

Several pictures of the subjectively most confusing parts:

<img src="images/1 SDK Tools.png" alt="Community Garden" width="600"/>
<img src="images/2 Device Manager.png" alt="Community Garden" width="600"/>
<img src="images/3 Device Configuration.png" alt="Community Garden" width="600"/>
<img src="images/4 Phone.png" alt="Community Garden" width="600"/>
<img src="images/5 Main Screen.png" alt="Community Garden" width="200"/>


### URL of Git Repo
[Our Git Repository](https://github.com/sbaek293/Studygram/)

### Libraries and Frameworks
**Libraries**
- Unity Game Engine (core platform)
- Photon PUN 2 Library (multiplayer)
- Firebase Unity SDK (Firebase Realtime Database)
- TextMeshPro (text rendering)

**Other Tools**
- Unity Hub & Editor 
- VS Code (C# IDE)
- ChatGPT, Gemini (coding support)
- GitHub Repository (project hosting)

### Individual Reflections
#### Alua
I was collaborating with Sche-In in creating and researching the backend server (Firebase) and connecting it to the app. In the code itself, I contributed mostly to the quiz scene, specifically, initializing the database, creating and assigning groups (with Johan), avatar display and assignment, and polishing the UI to resemble the lo-fi prototype.\
This was my first experience with a shared Git, so in the beginning, I had a lot of difficulties trying to push and pull changes. There were some problems getting used to the Unity interface, C#, and Firebase, since it was my first time using them. But the biggest challenge in coding was making group assignments properly connect to the database and match users correctly, since it was the basis of every aspect of our app, so implementing it wrong would crash the whole functionality. Since Johan and I were working separately on the same thing (back-front), there was some trouble understanding each other’s code. The code worked only after many different iterations.\
One useful skill I have learned through this project is handling Git merge conflicts and working with shared repositories in general. 

#### Sche In
First, I guided other team members without experience in Unity and Git to help them get used to the system. My main tasks included the authentication system, the multiplayer system where members could see each other moving around, the settings menu, and the functionality of the card set and group session systems. I implemented the backend for these features and also designed the UI for them.\
Afterwards, Olesia helped me polish the card set and group session systems. Additionally, I connected the Firebase database system throughout the entire game. I also managed and planned the overall structure of our project. Lastly, I handled the building and deployment of the application.
The most difficult part was integrating the Firebase Database into Unity because it was my first time working with it. Maintaining data consistency between users and adding new features without breaking existing code was also challenging throughout the project. Furthermore, because Unity is not primarily designed for UI-based applications, implementing UI design in the game engine was difficult. It was also my first time actively using AI for programming, which introduced additional challenges when debugging AI-generated code and integrating it into Unity.\
Through this experience, I was able to learn a lot about using databases and saving data with JSON in Unity. Additionally, due to the asynchronous nature of social interactions, it was my first time working with event handling, including listening and notifying.

#### Johan
My primary contribution to the project encompassed both the aesthetic environment and core system mechanics. Visually, I established the game’s atmosphere by leveraging AI tools to generate cohesive sprites for the environment, icons, and background elements. I integrated these assets into Unity, specifically building the UI for the Garden, Quiz, and Single User scenes to ensure a consistent user experience.\
On the technical side, I implemented the core "Study Buddy" matching logic. This involved coding the profiling quiz and the backend system that groups students based on compatibility. I also developed the Daily Streak system to encourage user retention and created the Pet Controller to add interactivity within the garden. Furthermore, I programmed the dynamic visual progression for the garden, ensuring the environment evolves as the group levels up. Finally, I tied these distinct features together by building the main navigation system that connects all game scenes seamlessly.\
One significant challenge was mastering Unity's UI system, particularly creating dynamic, scrollable content (like the Group Matching list). Coming from a web development background, transitioning from CSS logic to Unity’s component based architecture, managing RectTransforms, Layout Groups, and Content Size Fitters was complex. I had to learn how to manipulate anchors and pivots precisely to ensure the interface scaled correctly on different screen sizes.\
I learned Asynchronous Programming (async/await) in C#. This was used for fetching group data and user profiles from the cloud without freezing the game.

#### Olesia
Initially, I was supposed to write the flashcards-related code. However, I had a mental breakdown during the previous week, so Sche In wrote most of it. I worked extensively on bug fixes for both card scheme and group sessions, adding logic to the implementation (i.e. >=2 participants for the group session). It was my first time working on a CS project of this scale, and I feel grateful for the team that managed the completion of our project on time despite my breakdown and supported me during my very first experience with team coding, C#, using a database, etc. I learned the basics of C# and improved my QA skills. The part I struggled the most with is using the Realtime Database, since prior to that, I only worked with local storage. I wrote the report because I didn't have the chance to contribute as much to the coding as others.