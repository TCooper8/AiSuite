namespace AiLib

module Actor =
    type ActorConfig = 
        {   name:       string;
        }

    type Msgs =
        | Kill
        | SpawnActor of ActorConfig

    [<AbstractClass>]
    type 'a Actor(config:ActorConfig) as this =
        let agent = new MailboxProcessor<'a>(fun inbox ->
            let rec loop() = async {
                let! msg = inbox.Receive() 
                this.Receive msg 
                return! loop()
            }
            loop()
        )

        abstract Receive: 'a -> unit

        member this.Agent = agent
        member this.Name = config.name
        member this.Start = agent.Start()