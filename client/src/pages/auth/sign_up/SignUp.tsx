/**
 * SignUp Component
 *
 * This component provides a sign-up form for users to register with their information.
 *
 * It uses the IUser interface to define the shape of the form data and defaultUser
 * for initializing the form with default values.
 *
 * @returns JSX.Element
 */
import React, { useEffect, useState } from "react";
import IUser from "../../../interfaces/auth/sign_up/IUser";
import defaultUser from "../../../samples/auth/sign_up/signup";
import { ValidateSignupData } from "../../../validators/auth/sign_up/validate_signup";
import Navbar from "../../../components/navbar/Navbar";
import SignUpService from "../../../services/auth/sign_up/SignUpService";
import useAuth from "../../../contexts/use_auth/UseAuth";
import { useNavigate } from "react-router-dom";
import LoginService from "../../../services/auth/login/LoginService";
import ILogin from "../../../interfaces/auth/login/ILogin";
import IToken from "../../../interfaces/auth/jwt/IToken";
import { saveTokenToLocalstorage } from "../../../services/jwt/JWTTokenizationService";
import { jwtDecode } from "jwt-decode";
import LoadingSpinner from "../../../components/spinner/LoadingSpinner";

const SignUp: React.FC = () => {
  // State variables to manage form data, image, and error messages
  const [formData, setFormData] = useState<IUser>(defaultUser);
  const [image, setImage] = useState<File | null>(null);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const navigate = useNavigate();
  const { isLoggedIn, setToken, setEmail } = useAuth();
  const [loading, setLoading] = useState<boolean>(true);
  const [submitting, setSubmitting] = useState<boolean>(false);
  useEffect(() => {
    setLoading(true);

    if (isLoggedIn) {
      navigate("/");
    }

    setLoading(false);
  }, [isLoggedIn, navigate]);

  // Function to handle changes in form inputs
  const handleChange = (
    event: React.ChangeEvent<HTMLInputElement>,
    key: keyof IUser
  ) => {
    setFormData({
      ...formData,
      [key]: event.target.value,
    });
  };

  // Function to handle image file change
  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files[0]) {
      setImage(event.target.files[0]);
      setFormData({
        ...formData,
        image: event.target.files[0],
      });
    }
  };

  // Function to handle form submission
  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    setSubmitting(true);

    // Reset errors
    setErrorMessage("");

    // Client-Side data verification
    const errors: string[] = ValidateSignupData(formData);

    if (errors.length === 0) {
      // Call service to pass data to API
      const success: boolean = await SignUpService(formData);

      if (success) {
        // login user if registration is done
        const login: ILogin = {
          email: formData.email,
          password: formData.password,
        };
        const token: IToken | null = await LoginService(login);

        if (token) {
          // Set token and change logged in state
          saveTokenToLocalstorage(token);
          setToken(token);

          // set email address
          const decodedToken: IToken = jwtDecode(token.token ?? "");
          setEmail(decodedToken.email ?? "");

          navigate("/");
        } else {
          setErrorMessage("Entered credentials are incorrect.");
        }
      } else {
        setErrorMessage("Entered email address is used by another user.");
      }
    } else {
      // Show all errors
      setErrorMessage((prevErrorMessage) => {
        let newErrorMessage = prevErrorMessage + "Check next fields: ";
        errors.forEach((error, index) => {
          newErrorMessage += error;
          if (index !== errors.length - 1) {
            newErrorMessage += ", ";
          } else {
            newErrorMessage += ".";
          }
        });
        return newErrorMessage;
      });
    }

    setSubmitting(false);
  };

  // Render the sign-up form
  return (
    <>
      {loading ? (
        <LoadingSpinner />
      ) : (
        <>
          <Navbar />
          <section className="relative flex flex-wrap lg:h-screen lg:items-center">
            <div className="w-full px-4 py-12 sm:px-6 sm:py-16 lg:w-1/2 lg:px-8 lg:py-24">
              <div className="mx-auto max-w-lg text-center">
                <h1 className="text-2xl font-bold sm:text-3xl text-primary-600">
                  Sign Up
                </h1>

                <p className="mt-4 text-gray-500">
                  Join us and discover endless possibilities!
                </p>
              </div>

              <form
                onSubmit={handleSubmit}
                className="mx-auto mb-0 mt-8 max-w-md space-y-4"
                encType="multipart/form-data"
              >
                <div className="flex space-x-4">
                  <div className="w-1/2">
                    <label htmlFor="firstName" className="sr-only">
                      First Name
                    </label>
                    <input
                      type="text"
                      value={formData.firstName}
                      onChange={(e) => handleChange(e, "firstName")}
                      className="w-full rounded-lg border-2 focus:outline-none focus:ring-0 focus:border-primary-500 p-3 text-sm shadow-sm"
                      placeholder="First Name"
                    />
                  </div>
                  <div className="w-1/2">
                    <label htmlFor="lastName" className="sr-only">
                      Last Name
                    </label>
                    <input
                      type="text"
                      value={formData.lastName}
                      onChange={(e) => handleChange(e, "lastName")}
                      className="w-full rounded-lg border-2 focus:outline-none focus:ring-0 focus:border-primary-500 p-3 text-sm shadow-sm"
                      placeholder="Last Name"
                    />
                  </div>
                </div>

                <div className="flex space-x-4">
                  <div className="w-1/2">
                    <label htmlFor="address" className="sr-only">
                      Address
                    </label>
                    <input
                      type="text"
                      value={formData.address}
                      onChange={(e) => handleChange(e, "address")}
                      className="w-full rounded-lg border-2  focus:outline-none focus:ring-0 focus:border-primary-500 p-3 text-sm shadow-sm"
                      placeholder="Address"
                    />
                  </div>
                  <div className="w-1/2">
                    <label htmlFor="city" className="sr-only">
                      City
                    </label>
                    <input
                      type="text"
                      value={formData.city}
                      onChange={(e) => handleChange(e, "city")}
                      className="w-full rounded-lg border-2  focus:outline-none focus:ring-0 focus:border-primary-500 p-3 text-sm shadow-sm"
                      placeholder="City"
                    />
                  </div>
                </div>

                {Object.entries(formData).map(([key, value]) => {
                  if (
                    [
                      "firstName",
                      "lastName",
                      "address",
                      "city",
                      "image",
                    ].includes(key)
                  ) {
                    return null; // Skip first name, last name, address, and city
                  }
                  return (
                    <div key={key}>
                      <label htmlFor={key} className="sr-only">
                        {key.charAt(0).toUpperCase() + key.slice(1)}
                      </label>
                      <input
                        type={
                          key === "email"
                            ? "email"
                            : key === "password"
                            ? "password"
                            : "text"
                        }
                        value={value}
                        onChange={(e) => handleChange(e, key as keyof IUser)}
                        className="w-full rounded-lg border-2  focus:outline-none focus:ring-0 focus:border-primary-500 p-3 text-sm shadow-sm"
                        placeholder={key.charAt(0).toUpperCase() + key.slice(1)}
                      />
                    </div>
                  );
                })}
                <div className="relative flex items-center">
                  <input
                    type="file"
                    onChange={handleImageChange}
                    className="hidden"
                    id="image"
                  />
                  <label
                    htmlFor="image"
                    className="select-none rounded-lg bg-gradient-to-tr from-orange-500 to-primary-600 py-2 px-4 text-center align-middle font-sans text-xs font-bold uppercase text-white shadow-md shadow-primary-900/10 transition-all hover:shadow-lg hover:shadow-primary-900/20 active:opacity-[0.85] disabled:pointer-events-none disabled:opacity-50 disabled:shadow-none inline-block mr-2"
                  >
                    Choose Image
                  </label>
                  <span className="text-sm text-primary-500 mr-2">
                    {image && image.name}
                  </span>
                  {image && (
                    <div className="mt-2 ml-auto">
                      <img
                        src={URL.createObjectURL(image)}
                        alt="Image Preview"
                        className="w-16 h-16 rounded-lg object-cover"
                      />
                    </div>
                  )}
                </div>

                {errorMessage && (
                  <p className="mt-4 text-primary-600">{errorMessage}</p>
                )}

                <div className="flex items-center justify-between bottom-0">
                  <p className="text-sm text-gray-500">
                    Already have an account?
                    <a
                      className="text-primary-600 hover:text-primary-700"
                      href="/login"
                    >
                      &nbsp;Log in
                    </a>
                  </p>

                  <button
                    type="submit"
                    disabled={submitting}
                    className="select-none rounded-lg bg-gradient-to-tr from-primary-600 to-primary-600 py-2 px-4 text-center align-middle font-sans text-sm font-bold text-white shadow-md shadow-primary-900/10 transition-all hover:shadow-lg hover:shadow-primary-900/20 active:opacity-[0.85] disabled:pointer-events-none disabled:opacity-50 disabled:shadow-none inline-block"
                  >
                    {submitting ? "Please wait..." : "Sign up"}
                  </button>
                </div>
              </form>
            </div>

            <div className="hidden lg:block relative h-64 w-full sm:h-96 lg:h-full lg:w-1/2">
              <img
                alt=""
                src="/images/login.jpg"
                className="absolute inset-0 h-full w-full object-cover"
              />
            </div>
          </section>
        </>
      )}
    </>
  );
};

export default SignUp;
