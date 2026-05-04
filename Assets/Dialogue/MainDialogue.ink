EXTERNAL waitForCharacterSelect()
EXTERNAL waitForTutorial01()

-> main

=== main ===
Welcome, you lucky little bastard! #Face:Grin

You've been randomly chosen to face a trial where you can gain a new body and start a new life! #Face:Smile

Wanna play?

+[Yes]
    Great! #Face:Grin
+[No]
    #ExitGame
    ->END


- I'll be the test administrator for these little games you're about to play

First, you must pick a vessel from one of these podiums #Face:Smile
Can't really play games with flimsy ghost mitts, now can you? #Face:Wink
If you survive, that'll be your new body #Face:Smile
So, pick one you like
Now,

~ waitForCharacterSelect()
CHOOSE <br> A <br> VESSEL #Face:SuperStare

And just to up the stakes, I've put an innocent soul inside each of these #Face:Evil
You get three strikes, each resulting in the loss of a vessel and the soul within

However, each success will reward you with some currency to spend later on #Face:Smile
And if you lose, I might be persuaded to let you keep your soul and body for a price #Face:Wink
Before we begin, do you need a refresher? #Face:Smile

+[Yes]
    Really? I just told you this... #Face:Confused
    -> introRepeat
+[No]
    Perfect! #Face:Grin
-> tutorial
    
= skipTarget    
-> END

=== introRepeat ===
Welcome, wandering soul, blah blah blah #Face:Deadpan
I'm the test administrator

You already picked a vessel, don't need to ask that again#Face:Stare

If you survive, that'll be your new body #Face:Deadpan
There's an innocent soul inside each vessel because I'm evil
Every time you mess up in the game a vessel gets destroyed
You get money each time you succeed
If you lose, I might let you keep your soul and body for a price

Need another refresher? #Face:Squint

+[Yes]
    Yeah, no #Face:Stare
+[No]
    Good #Face:Grin
    
- -> tutorial

=== gameover ===
Did you really think I'd let you go in exchange for gold? #Face:Grin
-> DONE

=== skipMain ===
-> main.skipTarget

=== tutorial ===
Alright, lets get you used to your new body #Face:Smile

~ waitForTutorial01()

Click on the buttons that pop up

Amazing, you're a natural #Face:Grin

Also, you see that red bar right under you? #Face:Smile
That's how much time you get for these

I'm being real nice, letting you have extra time for these practice ones you know #Face:Fresh

~ waitForTutorial01()

Now click on the bone a bunch #Face:Smile

It's not animated, but I'm clapping. Use your imagination #Face:Cat

You see that 0 next to the timer bar? #Face:Smile
That's how much money you have

You're poor #Face:Grin

And I'm not giving you any money for these practice tests, cry about it #Face:Fresh

~ waitForTutorial01()

Lastly, slice the heart in the direction shown #Face:Smile

I promise that heart didn't belong to anyone #Face:Evil

Now, lets start the games for real #Face:Grin

-> DONE

=== function waitForCharacterSelect() ===
    ~ return
    
=== function waitForTutorial01() ===
    ~ return
    
=== function endGame() ===
    ~ return