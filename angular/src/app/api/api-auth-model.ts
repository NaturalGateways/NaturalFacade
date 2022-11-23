import { CognitoAccessModel } from '../auth/cognito-model';
import { CurrentUserApiDto } from './api-auth-dto/current-user-api-dto';

export class ApiAuthModel
{
  user: CurrentUserApiDto;
  
  access: CognitoAccessModel;

  constructor(user: CurrentUserApiDto, access: CognitoAccessModel)
  {
    this.user = user;
    this.access = access;
  }
}
