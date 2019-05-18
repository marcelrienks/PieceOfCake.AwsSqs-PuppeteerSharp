# PieceOfCake.AwsSqs-PuppeteerSharp
[![Build Status](https://dev.azure.com/marcelrienks/PieceOfCake.AwsSqs-PuppeteerSharp/_apis/build/status/marcelrienks.PieceOfCake.AwsSqs-PuppeteerSharp?branchName=master)](https://dev.azure.com/marcelrienks/PieceOfCake.AwsSqs-PuppeteerSharp/_build/latest?definitionId=9&branchName=master)  
This is a (P)iece(O)f(C)ake or a POC to test both AWS SQS and Puppeteer.

## AWS SQS
There is a services project which has two API calls that allow you to post a 'site query' onto an AWS SQS

## Puppeteer
There is then a processor project that will load chromium, using Puppeteer, pre-load two tabs one with Bing and one with Google.
It will then subscribe to the AWS SQS mentioned above, and read a 'site query' messages off the queue one at a time. It will then process them, by entering the search criteria on the relevant search engine page, selecting search, and then taking a screenshot of the result
