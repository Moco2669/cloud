import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import useAuth from "../../contexts/use_auth/UseAuth";
import GetUserByEmail from "../../services/users/profile/GetAccountDataService";
import Navbar from "../../components/navbar/Navbar";
import IUser from "../../interfaces/users/user/IUser";
import emptyUser from "../../samples/users/user";
import { removeTokenFromLocalStorage } from "../../services/jwt/JWTTokenizationService";
import { ValidateUpdateData } from "../../validators/users/validate_new_user_info";
import UpdateUserInformationService from "../../services/users/update/UpdateService";
import LoadingSpinner from "../../components/spinner/LoadingSpinner";
import ISearchBarQueryProps from "../../interfaces/search/ISearchBarQuery";

const Profile: React.FC <ISearchBarQueryProps>= ({query, setQuery}) => {
  const { email, token, isLoggedIn, setEmail, setToken } = useAuth();
  const [userData, setUserData] = useState<IUser | null>(emptyUser);
  const [image, setImage] = useState<File | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [errorMessage, setErrorMessage] = useState<string>("");
  const [successMessage, setSuccessMessage] = useState<string>("");
  const [pageLoaded, setPageLoaded] = useState<boolean>(false);
  const navigate = useNavigate();
  const [submitting, setSubmitting] = useState<boolean>(false);

  useEffect(() => {
    setPageLoaded(true);
  }, []);

  useEffect(() => {
    setLoading(true);

    if (pageLoaded && !isLoggedIn) {
      navigate("/");
    }

    const fetchData = async () => {
      try {
        if (email) {
          const userData: IUser | null = await GetUserByEmail(
            email,
            token?.token
          );
          if (userData) {
            setUserData(userData);
            const response: Response = await fetch(userData.ImageBlobUrl);
            if (!response.ok) {
              throw new Error(`Failed to fetch image: ${response.status}`);
            }
            const blob: Blob = await response.blob();

            // Create a File object from the Blob
            const file = new File([blob], "", {
              type: "image/jpeg",
            });

            setImage(file);
          } else {
            setEmail("");
            removeTokenFromLocalStorage();
            setToken(null);
            navigate("/");
          }
        }
      } catch (error) {
        navigate("/404");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [email, token, navigate, isLoggedIn, pageLoaded, setEmail, setToken]);

  // Function to handle image file change
  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    if (event.target.files && event.target.files[0]) {
      setImage(event.target.files[0]);
    }
  };

  const handleChange = (
    event: React.ChangeEvent<HTMLInputElement>,
    key: keyof IUser
  ) => {
    if (userData) {
      setUserData({
        ...userData,
        [key]: event.target.value,
      });
    }
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    setSubmitting(true);
    // Validate form data and update user data
    // Client-Side data verification
    const errors: string[] = ValidateUpdateData(userData ?? emptyUser);

    if (errors.length === 0 && userData) {
      // Call service to pass data to API
      const success: boolean = await UpdateUserInformationService(
        userData,
        image,
        token?.token ?? ""
      );

      if (!success) {
        setErrorMessage("Entered data are incorrect.");
        setSuccessMessage("");
      } else {
        setErrorMessage("");
        setSuccessMessage("Your information has been updated.");
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

  return (
    <>
      <Navbar query={query} setQuery={setQuery}/>
      {loading ? (
        <LoadingSpinner />
      ) : (
        <section className="flex items-center justify-center h-screen bg-slate-200/90 -mt-16 pb-12">
          <div className="max-w-lg bg-white rounded-lg shadow-lg p-8">
            <h1 className="text-2xl font-bold text-center text-primary-600 mb-1">
              Profile
            </h1>
            <p className=" text-gray-500 text-center mb-8">
              Keep your information up-to-date
            </p>
            <form
              onSubmit={handleSubmit}
              className="space-y-4"
              encType="multipart/form-data"
            >
              <div className="space-y-2">
                <div className="flex space-x-4">
                  <input
                    type="text"
                    autoFocus
                    value={userData?.FirstName}
                    onChange={(e) => handleChange(e, "FirstName")}
                    className="w-1/2 rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                    placeholder="First Name"
                  />
                  <input
                    type="text"
                    value={userData?.LastName}
                    onChange={(e) => handleChange(e, "LastName")}
                    className="w-1/2 rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                    placeholder="Last Name"
                  />
                </div>
                <div className="flex space-x-4">
                  <input
                    type="text"
                    value={userData?.Address}
                    onChange={(e) => handleChange(e, "Address")}
                    className="w-1/2 rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                    placeholder="Address"
                  />
                  <input
                    type="text"
                    value={userData?.City}
                    onChange={(e) => handleChange(e, "City")}
                    className="w-1/2 rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                    placeholder="City"
                  />
                </div>
                <div className="flex space-x-4">
                  <input
                    type="text"
                    value={userData?.Country}
                    onChange={(e) => handleChange(e, "Country")}
                    className="w-1/2 rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                    placeholder="Country"
                  />
                  <input
                    type="text"
                    value={userData?.Phone}
                    onChange={(e) => handleChange(e, "Phone")}
                    className="w-1/2 rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                    placeholder="Phone"
                  />
                </div>
                <input
                  type="password"
                  value={userData?.Password}
                  onChange={(e) => handleChange(e, "Password")}
                  className="w-full rounded-lg border border-gray-300 focus:outline-none focus:ring-primary-500 focus:border-primary-500 p-3 text-sm"
                  placeholder="Password"
                />
              </div>
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
                  Change Image
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
                <p className="text-primary-600">{errorMessage}</p>
              )}
              {successMessage && (
                <p className="text-emerald-600 text-center">{successMessage}</p>
              )}
              <div className="flex justify-end items-end">
                <button
                  disabled={submitting}
                  type="submit"
                  className="bg-primary-600 text-white rounded-lg py-2 px-4 font-bold text-sm shadow-md hover:shadow-lg focus:outline-none focus:ring-2 focus:ring-primary-500 focus:ring-offset-2 transition-all"
                >
                  {submitting ? "Saving..." : "Update"}
                </button>
              </div>
            </form>
          </div>
        </section>
      )}
    </>
  );
};

export default Profile;
