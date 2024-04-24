import IToken from "../../interfaces/auth/jwt/IToken";

/**
 * Saves a JWT token to the local storage.
 * @param data The token object containing the JWT token.
 */
const saveTokenToLocalstorage = (data: IToken): void => {
  localStorage.setItem("token", data.token ?? "");
};

/**
 * Removes the JWT token from the local storage.
 */
const removeTokenFromLocalStorage = (): void => {
  localStorage.removeItem("token");
};

export { saveTokenToLocalstorage, removeTokenFromLocalStorage };