Table of Contents
=================

  * [Summary:](#summary)
  * [Required to Run:](#required-to-run)
    * [Optional](#optional)
  * [How to run?](#how-to-run)
  * [How to stop and cleanup?](#how-to-stop-and-cleanup)
  * [How to run tests?](#how-to-run-tests)
  * [How to populate DynamoDB?](#how-to-populate-dynamodb)
  * [How I approached this task?](#how-i-approached-this-task)
  * [.NET open source packages used in the project:](#net-open-source-packages-used-in-the-project)
  * [Why I picked up DynamoDB?](#why-i-picked-up-dynamodb)

*Created by [gh-md-toc](https://github.com/ekalinin/github-markdown-toc)*

### Summary

!["basic diagram"](/diagrams/basic_diagram.png)

Payments system API sample using [Form3 API](http://api-docs.form3.tech/) as domain example. Approach with one single API.
- code was written on Linux Mint using I3wm and vscode + cli scripts

### Required to Run
- [.net code sdk 2.1](https://dotnet.microsoft.com/download)
- [aws cli](https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-install.html)
  - to create dynamodb on localstack
- [docker](https://docs.docker.com/install/linux/docker-ce/binaries/#prerequisites) 
  - Docker Engine 17.12 CE (tested on 17.12.0-ce, build c97c6d6)
  - if you have older or newer version you will need to change **docker-compose.yml** file version based on [link](https://docs.docker.com/compose/compose-file/#compose-and-docker-compatibility-matrix)
##### Optional
- [entr](http://eradman.com/entrproject/), [ripgrep](https://github.com/BurntSushi/ripgrep)
  - to run make auto-test 
  - it runs unit tests whenever a file is changed

### How to run?

Go to main folder and
```
make start
```
This command:
- starts docker composed [localstack](https://github.com/localstack/localstack) in detached mode with stubbed **DynamoDB**.
- runs a script to create **payments** table on localstack DynamoDB
- starts dotnet application on port **5000** for http and **5001** for https

### How to stop and cleanup?
Go to main folder and
```
make stop
```
This command:
- removes localstack and cleans up images

### How to run tests?
Go to main folder and
```
make all-tests
```

This command:
- runs unit tests
- starts up localstack with dynamodb and runs a script to create payments table
- runs integration tests
- stops localstack with dynamodb

### How to populate DynamoDB?
Go to main folder and
```
make put-single
```
For one payment.

```
make put-multi
```
For multiple payments.

### How I approached this task?
- Started with empty repo nothing special here added .gitignore.
- Added placeholder generate app from dotnet new.
- Added unit test folder.
- Wired up basis unit tests running (make auto-test).
- Wired up basic integration tests with **LightBDD**.
- Started with discovering problem space by working on **json** parsing. At the beginning I don't want to start with UI layer or DB layer. Domain, core is most important. By parsing json I was able to figure out how the data looks like, what kind of validations I need and how to design this system. This will lead to potential ideas which DB to pick or how to structure the API. This is also the moment wher communication beetwen Engineer <-> Domain Expert starts to happen.
- Added first API using InMemDB (List :)) simulating some data layer.
- Wired up IOC and wrote first unit tests for controller.
- Wrote controller flow logic with unit tests with some basic BDD style integration tests (still using InMemDb).
- Wired up first method GET using DynamoDB on localstack.
- Moved on with DELETE, PUT, POST methods and Payments List GET.
- Refactorization Clean Up.
- Adding more integration tests - fixing bugs along the way with unit tests to cover them.
- Creating simple implementation of Domain validation using FluentValidation.

### .NET open source packages used in the project:
- [LightBDD](https://github.com/LightBDD/LightBDD)
  - opinionated but interesting approach to write simple BDD styled tests.
  - this was my first time using this package and I will definitely get back to it.
  - generates nice reports in html <img width="200px" heigth="100px" src="/images/light_bdd.png" />
- [XUnit](https://github.com/xunit/xunit)
  - my standard go to **Unit Testing** framework.
  - It doesn't have more complex Assertions like **NUnit** but this can be solved by adding **FluentAssertions**
- [FluentAssertions](https://github.com/fluentassertins/fluentassertions)
  - changes the way assertions are written, forcing you to use syntax **object.Should().Be(something)**
- [Moq](https://github.com/moq/moq)
  - my go to **Mocking** framework. It just works :)
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
  - Built in serialization provided in MVC.Core are ok but limiting.
- [FluentValidation](https://github.com/JeremySkinner/FluentValidation)
  - ASP.Net Core supports model binding and DataComponents attribute based validations, but using these can lead to MVC HTTP layer leaking out to Domain logic.
  - For me Domain is *sacred* and should be only about my business problem. UI, DB layers are kept away.
  - FluentValidation provides nice framework to create and wire up validations on your domain objects.

### Why I picked up a DynamoDB?

- I will pick up some document based database or object based one as I dont see a huge benefit of data normalization in this case when the operations are soo simple and based on correltion id - payment id
- If searching is needed i would project the document database to different form - eg reporting database or used even 'search optimised persistence stores' like elastic search if that would be the requirement.

Database choice:
- Which database to pick heh?
- At the moment requirements are quite simple - we just list, update, add and get 'objects'
- get is simple and based on correlation id -> payment id
- list is still simple and is based on correlation id organisation -> give me all payments for organisation
- at the moment I don't have any filter, options like give me a payment with this amount
- update is also simple per payment id only

- I am assuming no normalization and that i want to store payments as simple objects - documents.
- What kind of indexes i need? One is - per paayment id another one is per organisation id -> dont need more at the moment?
- Maybe one sort key per processign date
- Not sure what kind of traffic i can expect -> write vs read? assuming for this exercise that there will be more reads than writes -> as customer writes a payment once then polls (assuming no loopback with events or websockets) mutliple times to get the status. Ocassionaly building report by listing payments. So more reads than writes. Not sure how 'real time should be the system' and what kind of caching we need. 

- First choice : RDBSM
 - mysql or sql 
  + supports proper transactions that would let me simplify application level and synchronization and just let rdbsm do the work
  + get indexing support - i can create many different indexes + query optimization layer - can be usefull if quesries are 'complicated'
  + encryption at rest support?
  + I could use RDS or AURORA in AWS
  - at the moment i have no requirements for more complicated queries
  - if i dont use managed cluster or instance eg AWS (Aurora or RDS) i will have to deal for myself with the support -> plus manual sharding and replication - not sure if our team has experience with it
 
- Second Choice: DynamoDB
 + fully managed
 + i have a lot of experience with it
 + fresh from Reinvent on demand dynamo! (can be risky but no need to fine tune read writes capacity!! yey)
 + it magically 'works' and scales if you do you home work with proper partitioning
 + column wide schema less -> can be usefull but in this case maybe not the thing i want
 + i can extract most importants columns like payment - id, organisation id + processing date and keep the rest as a json blob
 + no need to wory about data replication, sharding etc
 + encryption at rest ( for aws centric company perfect choice with kms )
 + fine tunting for eventual or strong consistency
 - limitations on the 'data' footprint (one column value cannot reach certain amount of data)
 - partitioning strategy is really important as we want to avoid 'hot' nodes scenario that is making life difficult
 - Indexing is 'limited' - but for this scenario good enoung
 - API is 'complicated'
 - costs 'can' get out of hand

- Third Choice: MongoDB
 + it is no longer 'web scale' after last jepsen tests - it is a proper stable 'persistence store'
 + documents fit in perfectly the data i want to keep
 + atlas! - managed mongo looks really interesting
 + transactions (atomic operations on document but recently they added multi document transactions) - (no trasaanctions for clusters yet)
 - is Atlas scalable? hassle free enough?
 - no experience apart from simple hobby use cases years ago

### Other Notes
- HTTP for me is just a 'layer' that exposes my domain. So I started by thinking about my domain object - Payment.
- Usually I create classess and test code in the same file later moving them around to proper projects. That gives me fast feedback loop without context switching and thinking how to leverage file system to describe my code. I first experiment with my problem space and domain - discovering it before making decision how to properly encapsulate everything so that it makes sense.
- When writing code I use auto test scripts (entr + ripgrep) to enable continous testin when file change (similar to [wallaby.js](https://wallabyjs.com/) or [NCrunch](https://www.ncrunch.net/)).
- For Payment type initialy my instict was to use Enum but then I realised that if I pick up a document or relational persistent store - it doesn't really matter if I safe couple of bytes for not storing string. 
- For processing date I was a bit surprised why the date doesn't have any time zone specific information. Then I found that it is actually assuming format 'yyyy-MM-dd'.
- Initialy tests used versy simple ine memory database - a stub using 'List'. This speeds up initial development without context switching to DB layer.
- While building test scenarios for parser, I discovered that there are two ID's.
  - I made an assumption that Id of the metadata is the correct id to represent Id in our system and the one in the attributes is for 'external consumers id representaion and correlation'.
  - Ideally i would call the attributes 'ExternalPayment' and our main object InternalPayment - or figure out a different terminilogoy on how to differentatie beetwen them - int he form3 api reference i found that you fetch the payment using guid id not the 'integer' or stirng one 

### Things I would do with more time

- TODO: dont forget to prepars the code with static analysis code checkes like stylecop
- TODO: add compression and caching
- TODO: writes tests to verify cache and gzip
- TODO: throttling on the api - return 429 -> of course ideally this should be on a different layer -> api gateway or even things like incapsula WAF DDOS protection layer
- TODO: metrics with prometheus
- TODO: influxDB + grafana
- TODO: logs through something simple? (dont want to create elasticsearch and kibana)
- TODO: load balancer proxy on nginx + 3 instances for reliability -> of course using cloud i would use route53 and ALB + simple DNS for service discovery
- TODO: test if it builds on windows
- TODO: do a chaos engineering round of tests - both the app and the unit tets (break code and see what kind of messages does the test give is it understood enough?)
- TODO: add license file

TODO
API needs to provide:
- HTOES -> https://github.com/faniereynders/aspnetcore-hateoas
- Content negotation
- Security Tokens in header -> Fake Auth server? Option for dev machine -> and tests
- Swagger ??
- Concurrency? (use version on document)
- Add Logging + Metrics? Prometheus?
- Caching and discussion about what kinf of caching layer to add

LOAD TESTS! using k6 -> using my own personal project

HTOES: -> lib in .NET

### System I would build given 'infinite' amount of time and resources


[Table of Contents](#table-of-contents)
