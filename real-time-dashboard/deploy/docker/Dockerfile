FROM registry.digi.is/igaming-brands/affiliates/affiliate-platform/containers/build-images:affiliate-platform-dotnet-7-runtime
COPY artifacts/published-solution /app

EXPOSE 80
EXPOSE 443
EXPOSE 3000
EXPOSE 8080
EXPOSE 9093

RUN sed -i 's/DEFAULT@SECLEVEL=2/ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-SHA384:ECDHE-ECDSA-AES128-SHA256:ECDHE-RSA-AES256-SHA384:ECDHE-RSA-AES128-SHA256/g' /etc/ssl/openssl.cnf

RUN useradd -u 1010 appuser
RUN chown -R appuser /app

WORKDIR /app
USER appuser
ENTRYPOINT ["dotnet", "Analytics.Infrastructure.Service.dll"]