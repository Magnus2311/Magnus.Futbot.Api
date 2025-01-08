За да го пуснем в docker:

# 1. First, generate a dev certificate
dotnet dev-certs https -ep ${HOME}/.aspnet/https/aspnetapp.pfx -p "YourSecurePassword"
dotnet dev-certs https --trust

# 2. Stop and remove the existing container
docker rm -f futbot-api-container

# 3. Run the container with HTTPS configuration
docker run -d \
  -p 7191:80 \
  -p 7192:443 \
  -e ASPNETCORE_URLS="https://+:443;http://+:80" \
  -e ASPNETCORE_HTTPS_PORT=7192 \
  -e ASPNETCORE_Kestrel__Certificates__Default__Password="YourSecurePassword" \
  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx \
  -v ${HOME}/.aspnet/https:/https/ \
  --name futbot-api-container \
  futbot-api