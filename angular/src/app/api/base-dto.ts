export class BaseResponseDto<PayloadType> {
  Success: boolean = false;
  
  Payload: PayloadType | undefined;
}
