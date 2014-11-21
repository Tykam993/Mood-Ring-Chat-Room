Mood Ring Chatroom - Procjam 2014
===
Team Three Musketeers
=


What
===
As a part of a college class, I participated in the Procedural Content Jam (http://itch.io/jam/procjam).
Our idea is to create a chat room that has procedurally-generated graphics based on what its users type into chat.

Why
===
I like the idea of the environment changing based on unconscious decisions. When we speak, we often don't realize
what kind of language or tone we're using--especially in online chat. I thought it would be fun to objectively (and not-so-objectively) analyze what people are saying, and reflect that visually.

-Inner-workings-
The app is set up with the Model-View-Control pattern (http://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller).
The chat system represents the view, the user-input as way of interfacing with the Control, and the Model as the "emotional state" of the user.

-as of 11/20/2014-
- When a user inputs something into chat, the controller grabs the input, splits it into a list of single words, and removes duplicates.
- It then checks the MySQL database to see if each word has any related value to it. If not, it then interacts with Big Huge Thesaurus to get a list of synonyms of the input word.
- If any of the synonyms match our criteria for having an "emotional value," we add the input word associated with its "emotional value" to the MySQL database (this saves us costly GET requests to Big Huge Thesaurus' API--we get 1000/day).
- If any emotional value is found, we add it to our model
- The model is represented as a 2D Vector, using this dimensions of emotion as our inner model:
    http://en.wikipedia.org/wiki/File:Two_Dimensions_of_Emotion.gif.jpg
- At any point, the View can check the current state of the Model, which is returned as a 2D Vector.

Future
===
(As of 11/20/2014) we are using Big Huge Thesaurus' API and my MySQL server to compare user input to a hand-crafted list of "emotional" words. In future versions, I plan to implement sentiment analysis to really capture what kinds of feeling and emotions users are communicating.
