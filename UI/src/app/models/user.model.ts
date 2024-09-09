export class User {
  id?: number;
  firstName?: string;
  lastName?: string;
  userName?: string;
  password?: string;
  email?: string;
  isActive?: boolean;
  createdBy?: string;
  createdDate?: string;
  updatedBy?: string;
  updatedDate?: string;
  userRoles:any;

}

export interface UserTest {
  id: number;
  name: string;
  email: string;
  role: string;
}