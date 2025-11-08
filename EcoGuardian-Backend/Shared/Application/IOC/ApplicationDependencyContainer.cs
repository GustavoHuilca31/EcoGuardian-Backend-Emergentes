using EcoGuardian_Backend.Analytics.Application.Internal.CommandServices;
using EcoGuardian_Backend.Analytics.Application.Internal.OutboundServices;
using EcoGuardian_Backend.Analytics.Application.Internal.QueryServices;
using EcoGuardian_Backend.Analytics.Domain.Repositories;
using EcoGuardian_Backend.Analytics.Domain.Services;
using EcoGuardian_Backend.Analytics.Infrastructure.Persistence.EFC.Repositories;
using EcoGuardian_Backend.CRM.Application.Internal.CommandServices;
using EcoGuardian_Backend.CRM.Application.Internal.EventHandlers;
using EcoGuardian_Backend.CRM.Application.Internal.OutboundServices;
using EcoGuardian_Backend.CRM.Application.Internal.QueryServices;
using EcoGuardian_Backend.CRM.Domain.Services;
using EcoGuardian_Backend.IAM.Application.Internal.CommandServices;
using EcoGuardian_Backend.IAM.Application.Internal.OutboundServices;
using EcoGuardian_Backend.IAM.Application.Internal.QueryServices;
using EcoGuardian_Backend.IAM.Domain.Services;
using EcoGuardian_Backend.IAM.Infrastructure.Hashing.BCrypt.Services;
using EcoGuardian_Backend.IAM.Infrastructure.Tokens.JWT.Services;
using EcoGuardian_Backend.IAM.Interfaces.ACL;
using EcoGuardian_Backend.IAM.Interfaces.ACL.Service;
using EcoGuardian_Backend.OperationAndMonitoring.Application.Internal.CommandServices;
using EcoGuardian_Backend.OperationAndMonitoring.Application.Internal.OutboundServices;
using EcoGuardian_Backend.OperationAndMonitoring.Application.Internal.QueryServices;
using EcoGuardian_Backend.OperationAndMonitoring.Domain.Services;
using EcoGuardian_Backend.SubscriptionsAndPayment.Application.Internal.CommandServices;
using EcoGuardian_Backend.SubscriptionsAndPayment.Application.Internal.QueryServices;
using EcoGuardian_Backend.SubscriptionsAndPayment.Domain.Repositories;
using EcoGuardian_Backend.SubscriptionsAndPayment.Domain.Services;
using EcoGuardian_Backend.SubscriptionsAndPayment.Infrastructure.Persistence.EFC.Repositories;
using EcoGuardian_Backend.OperationAndMonitoring.Interfaces.ACL;
using EcoGuardian_Backend.OperationAndMonitoring.Interfaces.ACL.Service;
using EcoGuardian_Backend.Planning.Application.Internal.CommandServices;
using EcoGuardian_Backend.Planning.Application.Internal.QueryServices;
using EcoGuardian_Backend.Planning.Domain.Services;
using EcoGuardian_Backend.CRM.Application.Internal.OutboundServices;
using EcoGuardian_Backend.ProfilePreferences.Application.Internal.CommandServices;
using EcoGuardian_Backend.ProfilePreferences.Application.Internal.QueryServices;
using EcoGuardian_Backend.ProfilePreferences.Domain.Model.Services;
using EcoGuardian_Backend.ProfilePreferences.Domain.Repositories;
using EcoGuardian_Backend.ProfilePreferences.Domain.Services;
using EcoGuardian_Backend.ProfilePreferences.Infrastructure.Persistence.EFC.Repositories;
using EcoGuardian_Backend.ProfilePreferences.Interfaces.REST.ACL;
using EcoGuardian_Backend.ProfilePreferences.Interfaces.REST.ACL.Service;
using EcoGuardian_Backend.Resources.Domain.Repositories;
using EcoGuardian_Backend.Resources.Infrastructure.Persistence.EFC.Repositories;
using EcoGuardian_Backend.Resources.Application.Internal.CommandServices;
using EcoGuardian_Backend.Resources.Application.Internal.OutBoundServices;
using EcoGuardian_Backend.Resources.Application.Internal.QueryServices;
using EcoGuardian_Backend.Resources.Domain.Services;
using EcoGuardian_Backend.Resources.Interfaces.ACL;
using EcoGuardian_Backend.Resources.Interfaces.ACL.Service;
using EcoGuardian_Backend.Shared.Application.Internal.CloudinaryStorage;
using EcoGuardian_Backend.Shared.Application.Internal.CloudinaryStorage.Configuration;
using EcoGuardian_Backend.Shared.Infrastructure.Cloudinary;

namespace EcoGuardian_Backend.Shared.Application.IOC;

public static class ApplicationDependencyContainer 
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
    {
        // Register application services here
        // Example: services.AddScoped<IExampleService, ExampleService>();

        // Register command handlers
        services.AddScoped<IExternalPlantEvaluatedService, ExternalPlantEvaluatedService>();
        services.AddScoped<IPlantCommandService, PlantCommandService>();
        services.AddScoped<IPlantQueryService, PlantQueryService>();
        services.AddScoped<IWellnessStateCommandService, WellnessCommandService>();
        services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
        services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
        services.AddScoped<IPaymentCommandService, PaymentCommandService>();
        services.AddScoped<IPaymentQueryService, PaymentQueryService>();
        services.AddScoped<ISubscriptionTypeCommandService, SubscriptionTypeCommandService>();
        services.AddScoped<ISubscriptionStateCommandService, SubscriptionStateCommandService>();
        services.AddScoped<IQuestionCommandService, QuestionCommandService>();
        services.AddScoped<IAnswerCommandService, AnswerCommandService>();
        services.AddScoped<IAnswerQueryService, AnswerQueryService>();
        services.AddScoped<IQuestionQueryService, QuestionQueryService>();

        // External User Service for CRM
        services.AddScoped<IExternalUserServiceCRM, ExternalUserServiceCRM>();

        services.AddScoped<IProfileCommandService, ProfileCommandService>();
        services.AddScoped<IProfileQueryService, ProfileQueryService>();


        // EventHandler
        services.AddScoped<IAddedQuestionEventHandler, AddedQuestionEventHandler>();

        services.AddScoped<IRoleCommandService, RoleCommandService>();
        services.AddScoped<IUserCommandService, UserCommandService>();
        services.AddScoped<IUserQueryService, UserQueryService>();
        services.AddScoped<IIamContextFacade, IamContextFacade>();
        services.AddScoped<IMonitorinContextFacade, MonitorinContextFacade>();
        services.AddScoped<IExternalUserService, ExternalUserService>();
        services.AddScoped<IOrderCommandService, OrderCommandService>();
        services.AddScoped<IOrderStateCommandService, OrderStateCommandService>();
        services.AddScoped<IOrderQueryService, OrderQueryService>();

        // Analytics
        services.AddScoped<IMetricTypeCommandService, MetricTypeCommandService>();
        services.AddScoped<IMetricTypeRepository, MetricTypeRepository>();
        services.AddScoped<IMetricRegistryCommandService, MetricRegistryCommandService>();
        services.AddScoped<IMetricRegistryRepository, MetricRegistryRepository>();
        services.AddScoped<IMetricRegistryQueryService, MetricRegistryQueryService>();
        
        services.AddScoped<IDeviceQueryService, DeviceQueryService>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IDeviceCommandService, DeviceCommandService>();

        services.AddScoped<INotificationCommandService, NotificationCommandService>();
        services.AddScoped<INotificationQueryService, NotificationQueryService>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IExternalPlantService, ExternalPlantService>();

        services.AddScoped<INotificationServiceFacade, NotificationServiceFacade>();
        services.AddScoped<IExternalNotificationService, ExternalNotificationService>();
        services.AddScoped<ICloudinaryStorage, CloudinaryStorage>();
        services.AddScoped<IResourceContextFacade, ResourceContextFacade>();
        services.AddScoped<IExternalResourceService, ExternalResourceService>();
        
        services.AddScoped<IAggregationLevelCommandService, AggregationLevelCommandService>();
        services.AddScoped<IAggregationLevelRepository, AggregationLevelRepository>();
        // Register other application services as needed
        /*services.AddScoped<IExternalCustomerService, ExternalCustomerService>();*/

        return services;
    }
}
