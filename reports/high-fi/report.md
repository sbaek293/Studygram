# Studygram DPM 4 Report
Gamify Labs: Alua Kaliazhdarova, Johan Ronnquist, Sche In Baek, Olesia Bilyk
## Project Summary
University students study alone instead of meeting new people to study together. Our app gamifies the process and matches students to work on the coursework together, making the process of studying less isolated and more efficient. Our approach emphasizes long-term teamwork through matching within class based on learning type of students, smoothened group study sessions, and shared resources that all group members contribute to by collaborating on shared flashcard sets, which results in imprved learning habits and efficiency through collaboration.

### Instruction
**First Steps**
1. Enter your username at Welcome Screen. (We do not have passwords since the app is used for social studying and the worst malicious users could do would be study while pretending to be someone else and copy studying resources of others.)
1. You will be moved to the "Room Scene". From there you can access "Social Garden" and your "Profile" stats. You can also see your coins (earned by studying together) and progress (which is group progress so it will remain 0 unless you join a group).
1. When entering the Garden for the first time, you will be redirected to the Study Type Quiz.
1. The quiz consists of 8 Multiple Choice Questions. After finishing it you will be invited to join a group of at most three other people based on the compatibility of your studying preferences.
1. Finishing the quiz will give you a virtual avatar based on your type. From now on, it will be present on your Room and Garden screens. In the social Garden, you will also be able to see others' avatars to feel their presence. You can move around with WASD keys to spot your teammates if they are also currently online.

**Group Session**
1. From the Garden, you can click "Home" to go back to the Room. You can also click "Menu" (three horizontal lines) which is the menu of your social studying tfrom which you can navigate to group session window or flashcards window.
1. In the Group Session window, you will be able to see all currently active study sessions that you can join with one click. If there are none, the list will be empty.
1. You can press "+" to start your own session that will immediately be displayed on other people's lists of active sessions.
1. You will need at least one other person to join for the "Play" button to be enabled (otherwise it wouldn't be very social).
1. When you (the host) press "Play", the timer starts. The number of coins each of the participants will accumulate is based on the duration of the session.
1. You can pause and resume the session.
1. Once you end the session, it will end for everyone. On each participant's screen a pop-up will appear stating the experiece points and coins learned.
1. You can exit the Group Session window and go back to the social Garden by pressing the arrow in the top left of the screen.

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


**Flashcard Study Mode**
1. The card sets you purchased/created are shown in their colour and the ones you do not have access to are displayed in grey. You can click on them, see the price and proceed with the purchase or cancel. It is the only way to spend earned coins and this design choice is to prevent people from accessing others' resources without contributing. Contribution is just being an active member of the community, so both flashcard creation and group study session participation are contributions in that sense.
1. Click on a set to practice with it. You will have "Left Arrow" and "Right Arrow" buttons to navigate between cards of the set if there is more than one. 
1. "Definition" cards flip as the usual flashcards. For "MCQ", you can click on answer choices to find out if they are correct. Once you are done practicing, press "Back" (arrow) button at the top of your screen to return to the Card Menu.

**Other Features**
1. At this point you might be wondering what were the level and progress bar for.



<img src="images/group session.png" alt="Quiz Result 1" width="200"/>\
Above: [Study session log with a button to start a new session]




## Prototype
### URL of the Prototype

### URL of Git Repo
[Our Git Repository](https://github.com/sbaek293/Studygram/)

### Libraries and Frameworks
bla-bla-bla


### Individual Reflections
#### Alua

#### Sche In

#### Johan
My primary contribution to the project encompassed both the aesthetic environment and core system mechanics. Visually, I established the game’s atmosphere by leveraging AI tools to generate cohesive sprites for the environment, icons, and background elements. I integrated these assets into Unity, specifically building the UI for the Garden, Quiz, and Single User scenes to ensure a consistent user experience.
On the technical side, I implemented the core "Study Buddy" matching logic. This involved coding the profiling quiz and the backend system that groups students based on compatibility. I also developed the Daily Streak system to encourage user retention and created the Pet Controller to add interactivity within the garden. Furthermore, I programmed the dynamic visual progression for the garden, ensuring the environment evolves as the group levels up. Finally, I tied these distinct features together by building the main navigation system that connects all game scenes seamlessly.

#### Olesia
