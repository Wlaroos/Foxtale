EXTERNAL waitForCondition()

-> main

=== main ===
#Face:Smile
Welcome, wandering soul, to the trial you must endure to regain your corporeal form.

I've been assigned as the test administrator for these little games.

#Face:Grin
First, you must inhabit a vessel from one of these podiums to proceed.

Can't really play games with flimsy ghost mitts, now can you?

#Face:Smile
If you survive, that'll be your new body.

#Face:Stare
Now,

~ waitForCondition()
CHOOSE <br> A <br> VESSEL

// + [Walm]
// + [Runic]
// + [Frank]

// #Face:Grin
// - Great choice, I like the bones on that one

#Face:Evil
And just to up the stakes, I've put an innocent soul inside each of these.

You get three strikes, each resulting in the loss of a vessel and the soul within.

#Face:Smile
However, each success will reward you with some currency to spend later on.

#Face:Grin
If you lose, I might be persuaded to let you keep your soul and body for a price.

#Face:Smile
Before we begin, do you need a refresher?

+[Yes]
    Really? I just told you this...
    -> main
+[No]
    Perfect!
    
-> END

=== gameover ===
#Face:Grin
Gold? You really thought you could just buy your way out with this?
-> DONE

=== function waitForCondition() ===
    ~ return