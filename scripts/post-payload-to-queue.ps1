$rabbitPayload = @{
    demoValue = "test1"
    demoTimestamp = (Get-Date -AsUTC -Format "yyyy-MM-ddTHH:mm:ss.fffZ")
} | ConvertTo-Json

rabbitmqadmin `
--host "localhost" `
--insecure `
--username "guest" `
--password "guest" `
--node "rabbitmq.cluster.local" `
publish message `
--routing-key "queue-x" `
--payload $rabbitPayload
