export class CognitoAccessModel
{
  idToken: string;
  refreshToken: string;
  accessToken: string;
  expiryDateTime: Date;

  constructor(idToken: string, refreshToken: string, accessToken: string, expiresIn: number)
  {
    this.idToken = idToken;
    this.refreshToken = refreshToken;
    this.accessToken = accessToken;
    this.expiryDateTime = new Date();
    this.expiryDateTime.setSeconds(this.expiryDateTime.getSeconds() + expiresIn);
    this.expiryDateTime.setMinutes(this.expiryDateTime.getMinutes() - 5);
  }
}
