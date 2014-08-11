Grooveshark Autoplayer
======================

A simple Windows Grooveshark client that automatically plays when other applications are quiet and automatically pauses when they're not. 

Built using the [Grooveshark Javascript API](http://developers.grooveshark.com/docs/js_api/) and the [CSCore audio library](http://cscore.codeplex.com/).

##Basic use case 
I'm listening to music but would like to do something else with audio in it (eg: watch a video). Normally, I'd pause the music, watch the video and then have to start the music again when the video stops. Instead, this client pauses the music as soon as the video start playing and then starts it again when the video stops again. 


##To use
You probably have to be using Windows 7/8. Steps are a little unintuitive at the moment:

1. Compile with Visual Studio
2. Launch client and add songs to your queue
3. File -> Tick "Play Automatically"
4. Hover over client's icon in the task bar and click the play button

   ![Instruction Image](https://raw.githubusercontent.com/samvrlewis/grooveshark-autoplayer/master/grooveshark-autoplay-play.png)
   
5. The client should pause when another another application makes audio and then play when no other applications are making audio. 
