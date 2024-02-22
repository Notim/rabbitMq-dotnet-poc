FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CompanyName.Ordering.Checkout.PaymentConfirmation.Worker/CompanyName.Ordering.Checkout.PaymentConfirmation.Worker.csproj", "CompanyName.Ordering.Checkout.PaymentConfirmation.Worker/"]
RUN dotnet restore "CompanyName.Ordering.Checkout.PaymentConfirmation.Worker/CompanyName.Ordering.Checkout.PaymentConfirmation.Worker.csproj"
COPY . .
WORKDIR "/src/CompanyName.Ordering.Checkout.PaymentConfirmation.Worker"
RUN dotnet build "CompanyName.Ordering.Checkout.PaymentConfirmation.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CompanyName.Ordering.Checkout.PaymentConfirmation.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyName.Ordering.Checkout.PaymentConfirmation.Worker.dll"]
