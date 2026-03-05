EXTERNAL waitForCharacterSelect()

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
    Perfect, lets start!
-> END
    
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
    
- -> END

=== gameover ===
#Face:Grin
Did you really think I'd let you go in exchange for gold?
-> DONE

=== skipMain ===
-> main.skipTarget

=== function waitForCharacterSelect() ===
    ~ return

=== function endGame() ===
    ~ return