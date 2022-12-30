export class CognitoAccessModel
{
  idToken: string;
  refreshToken: string;
  accessToken: string;
  expiryDateTime: string;

  constructor(idToken: string, refreshToken: string, accessToken: string, expiresIn: number)
  {
    this.idToken = idToken;
    this.refreshToken = refreshToken;
    this.accessToken = accessToken;
    var expiryDateTime : Date = new Date();
    expiryDateTime.setSeconds(expiryDateTime.getSeconds() + expiresIn - 5 * 60);
    this.expiryDateTime = expiryDateTime.toISOString();
  }
}
