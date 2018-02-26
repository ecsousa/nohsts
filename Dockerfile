###/
# BUILD STAGE
#/
FROM microsoft/dotnet:2.0-sdk as builder

RUN mkdir /src
WORKDIR /src/

# Copy just .sln and .csproj's and restore
# So that cache on rely on source code
COPY ./nohsts.sln /src/
COPY ./nohsts/nohsts.csproj /src/nohsts/nohsts.csproj 

RUN dotnet restore nohsts.sln -r debian-x64

# Main backend build
COPY . /src

WORKDIR /src/nohsts
RUN dotnet publish --no-restore -c release -o /build -r debian-x64

WORKDIR /src/

###/
# MAIN IMAGE STAGE
#/
FROM microsoft/dotnet:2.0-runtime-deps

WORKDIR /root
COPY --from=builder /build .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000/tcp

ENTRYPOINT ["./nohsts"]

