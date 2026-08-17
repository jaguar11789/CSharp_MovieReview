const API_BASE_URL = "https://localhost:7226";

document.addEventListener("DOMContentLoaded", async () => {

    const params  = new URLSearchParams(window.location.search);
    const movieId = params.get("id");

    console.log("현재 URL:", window.location.href);
    console.log("영화 ID:", movieId);

    if (!movieId)
    {
        console.error("영화 ID가 없습니다.");

        return;
    }

    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Movies/${movieId}`);

        if (!response.ok)
        {
            throw new Error(`영화 상세 정보 요청 실패 : ${response.status}`);
        }

        const movie = await response.json();
        console.log(movie);

        const reviewMovieInfo = document.getElementById("reviewMovieInfo");

        reviewMovieInfo.innerHTML = `
                                    <div class="review-movie-poster">
                                        <img
                                            src="https://image.tmdb.org/t/p/w500${movie.poster_path}"
                                            alt="${movie.title}"
                                        >
                                    </div>

                                    <div class="review-movie-detail">

                                        <span class="section-label">MOVIE REVIEW</span>

                                        <h1>${movie.title}</h1>

                                        <div class="review-movie-meta">
                                            <span>★ ${Number(movie.vote_average).toFixed(1)}</span>
                                            <span>${movie.release_date || "개봉일 미정"}</span>
                                        </div>

                                        <div class="review-movie-genres">
                                            ${movie.genres?.length
                                                ? movie.genres
                                                    .map(genre =>
                                                        `<span>${genre.name}</span>`
                                                    )
                                                    .join("")
                                                : ""
                                            }
                                        </div>

                                    </div>
                                `;
    }
    catch (error)
    {
        console.error("영화 상세 정보 요청 오류 : ", error);
    }
});