# esep-webhooks

AWS Lambda (.NET 10) that receives a GitHub `issues` webhook via API Gateway and posts
`Issue Created: <link>` to Slack. The Slack webhook URL is read from the `SLACK_URL`
Lambda environment variable (never committed).

Deploy: `cd EsepWebhook && dotnet lambda deploy-function EsepWebhook`
