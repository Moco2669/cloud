import React, { useEffect, useState, useRef } from "react";
import Navbar from "../../components/navbar/Navbar";
import IPost from "../../interfaces/post/view/IPost";
import PostPreview from "../../components/post/preview/PostPreview";
import GetPostsService from "../../services/post/read/ReadPostsService";

const Home: React.FC = () => {
  const [posts, setPosts] = useState<IPost[]>([]);
  const [id, setId] = useState('0');

  const lastPostRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetch = async () => {
      const response: IPost[] | null = await GetPostsService(id);
      if (response && response.length > 1) {
        setPosts(response);
        setId(response[response.length - 1].Id)
      }
    };

    fetch();
  }, []);

  useEffect(() => {
    const loadMorePosts = async () => {
      const newPosts: IPost[] | null = await GetPostsService(id);
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
      <Navbar />
      <br/>
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
