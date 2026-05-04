-> main

=== main ===
Color me impressed! #Face:Surprised
You actually survived! #Face:Grin
Not many can boast about that feat #Face:Wink
So, #Face:Grin
Want to sign a contract with me? #Face:Evil

+ [Yes]
    -> yes_path

+ [No] #KillPlayer
    -> END


=== yes_path ===
Okay, I know it's not... #Face:Fresh
Oh #Face:Shock
Well that was easy. #Face:Surprised
Guess I can skip my whole spiel. #Face:Happy
Just sign here. #Face:Grin

+ [Sign]
    I'm starting to like you. #Face:Wink
    See you soon! #Face:Evil #ExitGame
    -> END

+ [Fake Sign]
    Really? #Face:Stare
    -> fake_sign
    
=== fake_sign ===
Let's try that again #Face:Smile
Would you like to sign a contract with me? #Face:Evil
+ [Sign]
    There we go! #Face:Grin
    That wasn't so hard was it? #Face:Wink
    See you soon! #Face:Evil #ExitGame
    -> END

+ [Fake Sign]
... #Face:SuperStare #KillPlayer
-> END