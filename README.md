# mTLS net6

https://en.wikipedia.org/wiki/Mutual_authentication

Small test of mTLS with net6, based on: https://blog.kritner.com/2020/07/15/setting-up-mtls-and-kestrel/

### Initialize new project
```sh
dotnet new webapi
dotnet new editorconfig
dotnet new gitignore
dotnet add package Microsoft.AspNetCore.Authentication.Certificate
```

### Create client.ext
```sh
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
```

### Create CA and client cert

```sh
openssl genrsa -aes256 -out myCA.key 2048
openssl req -x509 -new -nodes -key myCA.key -sha256 -days 10240 -out myCA.pem
openssl x509 -outform der -in myCa.pem -out myCa.crt
openssl genrsa -out client.key 2048
openssl req -new -key client.key -out client.csr
openssl x509 -req -in client.csr -CA myCA.pem -CAkey myCA.key -CAcreateserial -out client.crt -days 1024 -sha256 -extfile client.ext
```

Do a `File->Import` of `myCA.crt` into **Keychain Access** (mac) under the System keychain
Store the new private client cert outside repo somewhere

### Configure Program.cs

- ClientCertificateMode.RequireCertificate
- Service CertificateAuthenticationEvents
- UseAuthentication

### Test mTLS on api with client cert

```sh
curl -s --key ~/.certs/mTLS/client.key --cert ~/.certs/mTLS/client.crt https://localhost:7222/weatherforecast | jq
```

