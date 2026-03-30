EXTERNAL waitForCharacterSelect()
EXTERNAL waitForTutorial01()

-> main

=== main ===
#Face:Smile
Welcome, you lucky little bastard!

You've been randomly chosen to face a trial where you can gain a new body and start a new life!

Wanna play?

+[Yes]
    Great!
+[No]
    #ExitGame
    ->END

- I'll be the test administrator for these little games you're about to play

#Face:Grin
First, you must pick a vessel from one of these podiums

Can't really play games with flimsy ghost mitts, now can you?

#Face:Smile
If you survive, that'll be your new body

So, pick one you like

Now,

~ waitForCharacterSelect()
#Face:Stare
CHOOSE <br> A <br> VESSEL

// + [Walm]
// + [Runic]
// + [Frank]

// #Face:Grin
// - Great choice, I like the bones on that one

#Face:Evil
And just to up the stakes, I've put an innocent soul inside each of these

You get three strikes, each resulting in the loss of a vessel and the soul within

#Face:Smile
However, each success will reward you with some currency to spend later on

#Face:Grin
If you lose, I might be persuaded to let you keep your soul and body for a price

#Face:Smile
Before we begin, do you need a refresher?

+[Yes]
#Face:Confused
    Really? I just told you this...
    -> introRepeat
+[No]
#Face:Grin
    Perfect!
-> tutorial
    
= skipTarget    
-> END

=== introRepeat ===
#Face:Smile
Welcome, wandering soul, blah blah blah

I'm the test administrator

#Face:Stare
You already picked a vessel, don't need to ask that again

#Face:Smile
If you survive, that'll be your new body

#Face:Evil
There's an innocent soul inside each vessel because I'm evil

Every time you mess up in the game a vessel gets destroyed

#Face:Smile
You get money each time you succeed

#Face:Grin
If you lose, I might let you keep your soul and body for a price

#Face:Smile
Need another refresher?

+[Yes]
#Face:Stare
    Yeah, no
+[No]
#Face:Grin
    Good
    
- -> tutorial

=== gameover ===
#Face:Grin
Did you really think I'd let you go in exchange for gold?
-> DONE

=== skipMain ===
-> main.skipTarget

=== tutorial ===
#Face:Smile
Alright, lets get you used to your new body

~ waitForTutorial01()


Click on the buttons that pop up

#Face:Grin
Amazing, you're a natural

#Face:Smile
Also, you see that red bar right under you?

That's how much time you get for these

#Face:Stare
I'm being real nice, letting you have extra time for these practice ones you know

~ waitForTutorial01()

#Face:Smile
Now click on the bone a bunch

#Face:Cat
It's not animated, but I'm clapping. Use your imagination

#Face:Smile
You see that fat 0 next to the timer bar I talked about earlier?

That's how much currency you have

#Face:Grin
You're poor

#Face:Stare
And I'm not giving you any money for these practice tests, cry about it

~ waitForTutorial01()

#Face:Smile
Lastly, slice the heart in the direction shown

#Face:Evil
I promise that heart didn't belong to anyone

#Face:Grin
Now, lets start the games for real

-> DONE

=== function waitForCharacterSelect() ===
    ~ return
    
=== function waitForTutorial01() ===
    ~ return
    
=== function endGame() ===
    ~ return