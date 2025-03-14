export enum UserStatus {
  New = "New",
  RevolutConfirmed = "RevolutConfirmed",
  RequisitionConfirmed = "RequisitionConfirmed",
  RequisitionExpired = "RequisitionExpired",
}

export interface AppUser {
  id: string;
  name: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  status: UserStatus;
}
