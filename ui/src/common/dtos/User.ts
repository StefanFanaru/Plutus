export enum UserStatus {
  New = "New",
  RevolutConfirmed = "RevolutConfirmed",
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
