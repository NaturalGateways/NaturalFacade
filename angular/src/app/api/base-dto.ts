export class BaseResponseDto<PayloadType> {
  Success: boolean = false;
  
  Payload: PayloadType | undefined;
}

export class BlankResponseDto {
  Success: boolean = false;
}
