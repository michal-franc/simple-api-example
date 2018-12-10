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
   * [Table of Contents](#table-of-contents)

Created by [gh-md-toc](https://github.com/ekalinin/github-markdown-toc)

### Summary:

!["basic diagram"](/diagrams/basic_diagram.png)

Payments system API sample using [Form3 API](http://api-docs.form3.tech/) as domain example. Approach with one single API.
- code was written on Linux Mint using I3wm and vscode + cli scripts

### Required to Run:
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

### Why I picked up DynamoDB?

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

MakeFile: 
- what are the options available
- what is entr
- what is rg

Picked frameworks Decisions:
- LightBDD -> i have most expewrience with specflow but this one is cumbersone to use and requires extensions to visual studio - i am mostly vim, visual studio code user so i had to find alternative and i found multiple frameworks -> picked up LightBDD (which was not easy to set up as their docs were lacking (but i pushed a PR with more information to their docs))
- Xunit
- FluenaAssertions
- LocalStack

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

 -> Things that need to invalidate payment
   -> Incorrect Type
   -> version -> ? how to use it?
   -> organisation_id -> if not existing error

   -> validations
      -> amount -> . vs , -> throw error no auto conversion -> assume only .
      -> check if is guid -> check if exists
      -> validate if payload ID was used or not

 -> When I post payment with same ID again it clashes and returns error
   -> should i return message that payment already exists? or could this be a security hole?
   -> as if someone gest a hold of it they can check if payments exists?
   -> ok assume that this is not a security hole and auth - org correlation is good enough

 -> When i Post to endpoint that doesnt exit i get 404
 -> When i put a proper payment i get 200
 -> When i ask for paayment info and it doesnt exist i get 404
 -> When i ask for exisitng payment i get the data and 200
 -> When i delete payment that doesnt exist i get 404
 -> When i delete payment that does exist i get 200
 -> When i delete payment that was already deleted i get 404
 -> when i post (update) payment that doesnt exist i get 404
 -> When i post (update) that exits i get 200

 -> Integraiton test for processing date -> how to deal with timezones?

- as a User of this API what kind of errors I would like to see?
- as a User of this API what kind of 'faults' I could make? (looking at payload)

-> create behaviours out of this -> codify them in the BDD styled tests
-> First list in TODO comments in the BDD styles tests

As i approached doing this.
- HTTP for me is just a 'layer' that exposes my domain.
- So i started by thinking aobut my domain object - Payment.
- Usually i create classess and test code in the same repo and then moving them around to proper projects. That gives my fast feedback loop without yet thinking on file system representation of my code. I first experiment with my problem space and domain - discovering it before making deicsion how to properly encapsulate everything so that it makes sense. As the domain here is simple i didnt have to do things like potential event storming and domain modelling with domain experts.
- I also use auto test scripts using entr that enables me to run unit tests quicklky when the file is changed (similar to wallaby.js or ncrunch)
- Provided json representation of domain for me is just a projection
- I will pick up some document based database or object based one as I dont see a huge benefit of data normalization in this case when the operations are soo simple and based on correltion id - payment id
- If searching is needed i would project the document database to different form - eg reporting database or used even 'search optimised persistence stores' like elastic search if that would be the requirement.
- This task is to provide api that is able to operate on 'Objects' per PaymentId
- For account tyupe and payment type initialy my instict was to use Enum but then i realised that if i picked up document or object persistent store - it doesn't really matter if i safe couple of bytes for not storing string. Also account type and payment type can be dynamic in the future and we might control what values are acceptable usiung different modularised logic instead of 'compiled type' typ safety check that would in the future require from us deploy of application to make changes on prod. Same with payment sub type and type
- For processing date i was a bit surprised why the date doesnt have any time zone specific informatin. assument that date in there is in UTC format and reflected this correctly in Domain.
- I created direct 'adapter' from http to domain. Didn't wanted to use reflection as it is slow and we need to optimize for write / read throughput.
- Started with Payment mapping as this was a great way for me to 'explore' how the payment object looks like and what kind of problems i can achieve. Created IPayment Mapper and first implementaiton for Json files (with this interface we are able to extend mapper to XML or orther formats)
- After mapping started working on simple BDD style tests to cover basic scenarios
- Initialy tests were suppoirted by versy simple inmemd 'DB' service that was just stubbing database behind the scenes.
- After establishing most of the scenarios in a text form - pseudo code I staaarted implementing them
- while building test scenarios i Discovered that tehere are two ids - id of the metadata and id of the payment
 - I made an assumption that id of the metadata is the correct id to represent Id in our system
   - and the one in the attributes is for 'external consumers id representaion and correlation'
- Ideally i would call the attributes 'ExternalPayment' and our main object InternalPayment - or figure out a different terminilogoy on how to differentatie beetwen them - int he form3 api reference i found that you fetch the payment using guid id not the 'integer' or stirng one 
- DEcided to use microsoft standard containeer for IOC -> No need to add cusotm open source one unless there is a need to -> i will start with the simple solution

- DynamoDB development
- https://github.com/justeat/LocalDynamoDb <- I could install this package from JE - we use it internally but it requires java so i decided to go with LocalStack


