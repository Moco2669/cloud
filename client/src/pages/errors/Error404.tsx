import React from "react";
import Navbar from "../../components/navbar/Navbar";
import ISearchBarQueryProps from "../../interfaces/search/ISearchBarQuery";

const Error404: React.FC <ISearchBarQueryProps>= ({query, setQuery}) => {
  return (
    <>
      <Navbar query={query} setQuery={setQuery}/>
      <div className="grid mt-16 place-content-center bg-white px-4">
        <div className="text-center">
            <img src="/images/reddit404b.png" className="w-full h-4/6" />
          <h1 className="mt-6 text-2xl font-bold tracking-tight text-gray-900 sm:text-4xl">
            Uh-oh!
          </h1>
          <p className="mt-4 text-gray-500">We can't find that page.</p>
          <a
            href="/"
            className="mt-6 inline-block bg-primary-600 uppercase rounded-full px-5 py-3 text-sm font-medium text-white hover:bg-primary-500 focus:outline-none focus:ring"
          >
            Go Back Home
          </a>
        </div>
      </div>
    </>
  );
};

export default Error404;
