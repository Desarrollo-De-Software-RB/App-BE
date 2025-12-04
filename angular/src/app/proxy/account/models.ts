
export interface AppRegisterDto {
    userName: string;
    emailAddress: string;
    password: string;
    name: string;
    surname: string;
    phoneNumber?: string;
    profilePicture?: string;
}

export interface AppUpdateProfileDto {
    userName: string;
    email: string;
    name: string;
    surname: string;
    phoneNumber: string;
    profilePicture?: string;
}
