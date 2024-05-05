import React, { useEffect, useState, useRef } from "react";
import Navbar from "../../components/navbar/Navbar";
import IPost from "../../interfaces/post/view/IPost";
import PostPreview from "../../components/post/preview/PostPreview";
import GetPostsService from "../../services/post/read/ReadPostsService";
import ISearchBarQueryProps from "../../interfaces/search/ISearchBarQuery";
import useAuth from "../../contexts/use_auth/UseAuth";
import DropdownMenu from "../../components/dropdownmenu/DropdownMenu";
import DropdownMenuPoster from "../../components/dropdownmenu/DropdownMenuPoster";
import GetUsersPostsService from "../../services/post/read/ReadUsersPostsService";

const Home: React.FC<ISearchBarQueryProps> = ({query, setQuery}) => {
  const [posts, setPosts] = useState<IPost[]>([]);
  const [id, setId] = useState("0");
  const { email } = useAuth();
  const {token} = useAuth();
  const [sort, setSort] = useState<number>(0);
  const [time, setTime] = useState<Date>(new Date());
  const lastPostRef = useRef<HTMLDivElement>(null);
  const [ userOrAll, setUserOrAll] = useState<boolean>(false);

  useEffect(() => {
    const fetch = async () => {
      setTime(new Date());
      let response: IPost[] | null;
      
      if(!userOrAll){
        if(query === ""){
          setQuery("~");
          response = await GetPostsService("0", "~", sort, time);
        }
        else{
          response = await GetPostsService("0", query, sort, time);
        }
      }
      else{
        if(query === ""){
          setQuery("~");
          response = await GetUsersPostsService("0", "~", sort, time, email ?? "", token?.token ?? "");
        }
        else{
          response = await GetUsersPostsService("0", query, sort, time, email ?? "", token?.token ?? "");
        }
      }
      
      if (response && response.length > 0) {
        setPosts(response);
        setId(response[response.length - 1].Id);
      }
    };

    fetch();
  }, [query, sort, userOrAll]);

  useEffect(() => {
    const loadMorePosts = async () => {
      let newPosts: IPost[] | null;

      if(!userOrAll){
        if(query === ""){
          setQuery("~");
          newPosts = await GetPostsService(id, "~", sort, time);
        }
        else{
          newPosts = await GetPostsService(id, query, sort, time);
        }
      }
      else{
        if(query === ""){
          setQuery("~");
          newPosts = await GetUsersPostsService(id, "~", sort, time, email ?? "", token?.token ?? "");
        }
        else{
          newPosts = await GetUsersPostsService(id, query, sort, time, email ?? "", token?.token ?? "");
        }
      }
      
      if (newPosts && newPosts.length > 0) {
        setPosts(prevPosts => [...prevPosts, ...newPosts]);
        setId(newPosts[newPosts.length - 1].Id);
      }
    };

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadMorePosts();
        }
      },
      {
        root: null,
        rootMargin: '0px',
        threshold: 0.1 
      }
    );

    if (lastPostRef.current) {
      observer.observe(lastPostRef.current);
    }

    return () => {
      if (lastPostRef.current) {
        observer.unobserve(lastPostRef.current);
      }
    };
  }, [id]);

  return (
    <>
      <Navbar setQuery={setQuery} query={query}/>
      <br/>
      <div className="flex justify-end mx-48 relative mb-8">
      {email && (
    <div className="mr-2">
      <DropdownMenuPoster setUserOrAll={setUserOrAll} />
    </div>
  )}
  <div className="mr-2">
    <DropdownMenu setSort={setSort} />
  </div>
</div>
      {/* all current posts  */}
      {posts.map((post: IPost, index: number) => (
        <div className="mx-48"
          key={index}
        >
          <PostPreview post={post} />
          {index === posts.length - 1 && <div ref={lastPostRef}></div>}
        </div>
      ))}
    </>
  );
};

export default Home;