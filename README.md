# mTLS


Test of mTLS, based on post: https://blog.kritner.com/2020/07/15/setting-up-mtls-and-kestrel/

### Initialize
```sh
dotnet new webapi
dotnet add package Microsoft.AspNetCore.Authentication.Certificate
```

### Create CA and client cert

Import myCA into keychain (mac) and store client cert privkey outside repo or in keyvault

```sh
openssl genrsa -aes256 -out myCA.key 2048
openssl req -x509 -new -nodes -key myCA.key -sha256 -days 10240 -out myCA.pem\n
openssl x509 -outform der -in myCa.pem -out myCa.crt
openssl genrsa -out client.key 2048
openssl req -new -key client.key -out client.csr
openssl x509 -req -in client.csr -CA myCA.pem -CAkey myCA.key -CAcreateserial -out client.crt -days 1024 -sha256 -extfi
```

### Configure Program.cs

- ClientCertificateMode.RequireCertificate
- Service CertificateAuthenticationEvents
- UseAuthentication

### Test mTLS

```sh
curl -s --key ~/.certs/mTLS/client.key --cert ~/.certs/mTLS/client.crt https://localhost:7222/weatherforecast | jq
```

