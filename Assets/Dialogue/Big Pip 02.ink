-> main

=== main ===
Want to sign a contract with me? #Face:Grin

+ [Yes]
Okay, I know it's not... #Face:Fresh
Oh #Face:Shock
Well that was easy. #Face:Surprised
Guess I can skip my whole spiel. #Face:Happy
Just sign here. #Face:Grin
    
    ++ [Sign]
    I'm starting to like you. #Face:Wink
    Now get your ass back into those games! #Face:Grin
    And don't die, that would make this whole thing a waste of time. #Face:Cat
    -> END

    ++ [Fake Sign]
    Really? #Face:Stare

        +++ [Sign]
        That's what I thought. #Face:Evil
        -> END
        
        +++ [Fake Sign]
        ... #Face:SuperStare
        
            ++++ [:D]
            -> END
            
            ++++ [My bad]
            #KillPlayer
            -> END

+ [No]
Aw come on, why not?
-> END
    
+ [Depends]
Damn, I thought you were the type of person to blindly accept terms and services.
-> END
    
+ [Why?]
Why not?
-> END
    
= skipTarget
-> END

=== skipMain ===
-> main.skipTarget