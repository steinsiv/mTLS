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
# Create RSA private key for myCA
openssl genrsa -aes256 -out myCA.key 2048
openssl rsa -in myCA.key -noout -text       # inspect RSA private key

# Create CACert in PEM format
openssl req -x509 -new -nodes -key myCA.key -sha256 -days 10240 -out myCACert.pem -config myCACert.conf
openssl x509 -in myCACert.pem -noout -text  # inspect x509 cert

# Create CSR certificate signing request
openssl genrsa -out client.key 2048
openssl req -new -key client.key -out client.csr -config client.conf
openssl req -noout -text -in client.csr     # inspect certificate signing request CSR

# Create client cert signed with myCA
openssl x509 -req -in client.csr -CA myCACert.pem -CAkey myCA.key -CAcreateserial -out client.crt -days 1024 -sha256 -extfile client.ext 
openssl x509 -in client.crt -noout -text    # inspect x509 client cert
```

### Configure Program.cs

- Configure ClientCertificateMode.RequireCertificate
- Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme).AddCertificate
- app.UseAuthentication();

### Test mTLS

```sh
curl -s --key ./client.key --cert ./client.crt https://localhost:7222/weatherforecast | jq
```


