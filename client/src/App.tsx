import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Login from "./pages/auth/login/Login";
import Register from "./pages/auth/sign_up/SignUp";
import Home from "./pages/home/Home";
import Profile from "./pages/profile/Profile";
import Create from "./pages/post/create/Create";
import Error404 from "./pages/errors/Error404";
import Post from "./pages/post/view/Post";
import { useState } from "react";

function App() {
  const [query, setQuery] = useState<string>('~');

  return (
    <>
      <Router>
        <Routes>
          <Route path="/" element={<Home query={query} setQuery={setQuery} />} />
          <Route path="/post/:id" element={<Post query={query} setQuery={setQuery}/>} />
          <Route path="/create" element={<Create query={query} setQuery={setQuery}/>} />
          <Route path="/profile" element={<Profile query={query} setQuery={setQuery}/>} />
          <Route path="/login" element={<Login query={query} setQuery={setQuery} />} />
          <Route path="/signup" element={<Register query={query} setQuery={setQuery} />} />

          <Route path="*" element={<Error404 query={query} setQuery={setQuery}/>} />
        </Routes>
      </Router>
    </>
  );
}

// Global API endpoint
export const API_ENDPOINT: string = "http://localhost/api/"

export default App;
