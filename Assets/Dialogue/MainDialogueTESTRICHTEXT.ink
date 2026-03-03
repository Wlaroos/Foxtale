// <color=\#59DFF5> -- cyan


EXTERNAL waitForCharacterSelect()

-> main

=== main ===
#Face:Smile
Welcome, you <color=green>lucky</color> little bastard!

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

Can't really play games with <color=\#59DFF5>flimsy ghost mitts</color>, now can you?

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
And just to up the stakes, I've put an innocent <color=\#59DFF5>soul</color> inside each of these

You get <color=red>three strikes</color>, each resulting in the <color=red>loss</color> of a vessel and the <color=\#59DFF5>soul</color> within

#Face:Smile
However, each <color=green>success</color> will reward you with some <color=yellow>currency</color> to spend later on

#Face:Grin
If you <color=red>lose</color>, I might be persuaded to let you keep your <color=\#59DFF5>soul</color>  and body for a <color=yellow>price</color>

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

=== introRepeat ===
#Face:Smile
Welcome, wandering <color=\#59DFF5>soul</color>, blah blah blah

I'm the test administrator

#Face:Stare
You already picked a vessel, don't need to ask that again

#Face:Smile
If you <color=green>survive</color>, that'll be your new body

#Face:Evil
There's an innocent <color=\#59DFF5>soul</color> inside each vessel because I'm <color=red>evil</color>

Every time you <color=red>mess up</color> in the game a vessel gets <color=red>destroyed</color> 

#Face:Smile
You get <color=yellow>money</color> each time you <color=green>succeed</color>

#Face:Grin
If you <color=red>lose</color>, I might let you keep your <color=\#59DFF5>soul</color> and body for a <color=yellow>price</color>

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
Did you really think I'd let you go in exchange for <color=yellow>gold</color>?
-> DONE

=== function waitForCharacterSelect() ===
    ~ return
    
=== function endGame() ===
    ~ return