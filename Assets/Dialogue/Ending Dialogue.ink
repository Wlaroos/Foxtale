-> main

=== main ===
Want to sign a contract with me? #Face:Grin

+ [Yes]
    -> yes_path

+ [No]
    Your Loss
    #KillPlayer
    -> DONE


=== yes_path ===
Okay, I know it's not... #Face:Fresh
Oh #Face:Shock
Well that was easy. #Face:Surprised
Guess I can skip my whole spiel. #Face:Happy
Just sign here. #Face:Grin

+ [Sign]
    I'm starting to like you. #Face:Wink
    Now get your ass back into those games! #Face:Grin
    And don't die, that would make this whole thing a waste of time. #Face:Cat
    -> END

+ [Fake Sign]
    Really? #Face:Stare
    #KillPlayer
    -> END