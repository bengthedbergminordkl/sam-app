# sam-app

An example app that is based on [James Eastham video](https://www.youtube.com/watch?v=UEsyrtu64ZA&t=82s).

A REST API with a Lambda that accepts a GET resuest with query parameters. 
Push an event onto an event bridge that has one rule that logs all events to a cloudwatch log.

The sample is to test Lambda and EventBridge schemas using xUnit theories.

Additional Resources:

* Generate test events with sam local - https://docs.aws.amazon.comserverles   
* Serverless Test Samples - https://github.com/aws-samples/server...
* xUnit Theories - https://hamidmosalla.com/2017/02/25/x...
* NJsonSchema Library - https://github.com/RicoSuter/NJsonSchema

## Build

To build and deploy your application for the first time, run the following in your shell:


```bash
dotnet test
dotnet build
sam build
sam deploy --guided
```

## Test and Monitor

Show Log

 `aws logs tail /aws/events/HelloWorldBus --tail`

 Execute a request

 `curl https://940foi3sp5.execute-api.ap-southeast-2.amazonaws.com/Prod/hello?message=hello`







