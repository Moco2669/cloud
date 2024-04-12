import IUser from "../../../interfaces/auth/sign_up/IUser";

/**
 * Default sign up credentials.
 */
const defaultUser: IUser = {
    firstName: "",
    lastName: "",
    address: "",
    city: "",
    country: "",
    phone: "",
    email: "",
    password: "",
    image: null,
};

export default defaultUser;
