using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NaturalFacade.ApiLambdas;

public class Functions
{
    /// <summary>Handler for a request without authentication.</summary>
    public async Task<ApiDto.ApiResponseDto> AnonFunctionHandler(ApiDto.AnonRequestDto request, ILambdaContext context)
    {
        // Create service
        Natural.Aws.IAwsService awsService = new Natural.Aws.LambdaAwsService();
        using (Natural.Aws.DynamoDB.IDynamoService? dynamoDb = awsService.CreateDynamoService())
        {
            try
            {
                Services.DynamoService dynamoService = new Services.DynamoService(dynamoDb, DynamoTableNames.Singleton);
                object responseObj = await Services.ApiService.HandleAnonRequestAsync(dynamoService, request.payload);
                return ApiDto.ApiResponseDto.CreateSuccess(responseObj);
            }
            catch (FacadeApiException rae)
            {
                return ApiDto.ApiResponseDto.CreateError(rae.UserMessage, rae);
            }
            catch (Exception ex)
            {
                return ApiDto.ApiResponseDto.CreateError("Internal error.", ex);
            }
        }
    }

    /// <summary>Handler for a request by an authenticated user.</summary>
    public async Task<ApiDto.ApiResponseDto> AuthFunctionHandler(ApiDto.AuthRequestDto request, ILambdaContext context)
    {
        // Create service
        Natural.Aws.IAwsService awsService = new Natural.Aws.LambdaAwsService();
        using (Natural.Aws.DynamoDB.IDynamoService? dynamoDb = awsService.CreateDynamoService())
        {
            try
            {
                Services.DynamoService dynamoService = new Services.DynamoService(dynamoDb, DynamoTableNames.Singleton);
                object responseObj = await Services.ApiService.HandleAuthRequestAsync(dynamoService, request);
                return ApiDto.ApiResponseDto.CreateSuccess(responseObj);
            }
            catch (FacadeApiException rae)
            {
                return ApiDto.ApiResponseDto.CreateError(rae.UserMessage, rae);
            }
            catch (Exception ex)
            {
                return ApiDto.ApiResponseDto.CreateError("Internal error.", ex);
            }
        }
    }
}
