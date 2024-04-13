import React, { useEffect, useState, useRef } from "react";
import Navbar from "../../components/navbar/Navbar";
import IPost from "../../interfaces/post/view/IPost";
import PostPreview from "../../components/post/preview/PostPreview";
import GetPostsService from "../../services/post/read/ReadPostsService";

const Home: React.FC = () => {
  const [posts, setPosts] = useState<IPost[]>([]);
  const [pageNumber, setPageNumber] = useState(1); // Initialize page number
  const [pageSize] = useState(5); // Define page size

  const lastPostRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const fetchPosts = async () => {
      const response: IPost[] | null = await GetPostsService(
        pageNumber,
        pageSize
      );
      if (response && response.length > 0) {
        const filtered: IPost[] = [... new Set(posts.concat(response))]

        setPosts(filtered);
      }
    };

    fetchPosts();
  }, [pageNumber, pageSize]); // Trigger effect when page number or page size changes

  useEffect(() => {
    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          loadMorePosts();
        }
      },
      {
        root: null,
        rootMargin: "0px",
        threshold: 0.1,
      }
    );

    if (lastPostRef.current) {
      observer.observe(lastPostRef.current);
    }

    return () => {
      // if (lastPostRef.current) {
      //   observer.unobserve(lastPostRef.current);
      // }
    };
  }, [posts, pageNumber]); // Observe changes to the posts array

  const loadMorePosts = () => {
    setPageNumber((prevPageNumber) => prevPageNumber + 1); // Increment page number to load next page
  };

  return (
    <>
      <Navbar />
      <br />
      {/* Render posts */}
      {posts.map((post: IPost, index: number) => (
        <div className="mx-48" key={index}>
          <PostPreview post={post} />
          {index === posts.length - 1 && <div ref={lastPostRef}></div>}
        </div>
      ))}
    </>
  );
};

export default Home;
