import React, { useEffect, useState } from "react";
import Navbar from "../../../components/navbar/Navbar";
import CreatePostForm from "../../../components/post/create/CreatePost";
import useAuth from "../../../contexts/use_auth/UseAuth";
import { useNavigate } from "react-router-dom";
import ISearchBarQueryProps from "../../../interfaces/search/ISearchBarQuery";

const Create: React.FC <ISearchBarQueryProps>= ({query, setQuery}) => {
  const { isLoggedIn } = useAuth();
  const navigate = useNavigate();
  const [pageLoaded, setPageLoaded] = useState<boolean>(false);

  useEffect(() => {
    // Set pageLoaded to true once the page is loaded
    setPageLoaded(true);
  }, []);

  useEffect(() => {
    // Check isLoggedIn only after the page is loaded
    if (pageLoaded) {
      if (!isLoggedIn && pageLoaded) {
        navigate("/");
      }
    }
  }, [pageLoaded, isLoggedIn, navigate]);
  
  return (
    <>
      <Navbar query={query} setQuery={setQuery}/>
      <div className="bg-slate-200/90 py-8 min-h-screen">
        <CreatePostForm />
      </div>
    </>
  );
};

export default Create;
