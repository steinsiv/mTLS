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
# Create RSA private key
openssl genrsa -aes256 -out myCA.key 2048
openssl rsa -in myCA.key -noout -text       # inspect

# Create CACert in PEM and DER format
openssl req -x509 -new -nodes -key myCACert.key -sha256 -days 10240 -out myCACert.pem
openssl x509 -outform der -in myCACert.pem -out myCACert.crt
openssl x509 -in myCACert.pem -noout -text  # inspect

# Create CSR
openssl genrsa -out client.key 2048
openssl req -new -key client.key -out client.csr
openssl req -noout -text -in client.csr     # inspect

# Create client cert chained with CA
openssl x509 -req -in client.csr -CA myCA.pem -CAkey myCA.key -CAcreateserial -out client.crt -days 1024 -sha256 -extfile client.ext

```

### Configure Program.cs

- Configure ClientCertificateMode.RequireCertificate
- Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate
- app.UseAuthentication();

### Test mTLS

```sh
curl -s --key ~/.certs/mTLS/client.key --cert ~/.certs/mTLS/client.crt https://localhost:7222/weatherforecast | jq
```


