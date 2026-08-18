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
    // 정렬 버튼 이벤트
    const latestSortButton = document.getElementById("latestSortButton");
    const ratingSortButton = document.getElementById("ratingSortButton");

    latestSortButton.addEventListener("click", async () => {

        setReviewSortActive("latest");

        await loadReviews(movieId, "latest");
    });

    ratingSortButton.addEventListener("click", async () => {

        setReviewSortActive("rating");

        await loadReviews(movieId, "rating");
    });

    // 영화 정보
    await loadMovie(movieId);

    // 기본 정렬 : 최신순
    await loadReviews(movieId, "latest");
});

async function loadMovie(movieId)
{
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
        const reviewSubmitButton = document.getElementById("reviewSubmitButton");

        reviewSubmitButton.addEventListener("click", () => {
            createReview(movieId);
        });
    }
    catch (error)
    {
        console.error("영화 상세 정보 요청 오류 : ", error);
    }
}

async function loadReviews(movieId, sort = "latest")
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Reviews/movie/${movieId}?sort=${sort}`);

        if (!response.ok)
        {
            throw new Error(`리뷰 목록 요청 실패 : ${response.status}`);
        }

        const reviews = await response.json();

        console.log("리뷰 목록 : ", reviews);

        renderReviews(reviews);
    }
    catch (error)
    {
        console.error("리뷰 목록 조회 오류:", error);
    }
}

function renderReviews(reviews)
{
    const reviewList = document.getElementById("review-list");

    if (!reviews || reviews.length === 0)
    {
        reviewList.innerHTML = `<div class="review-empty">
                                    아직 등록된 리뷰가 없습니다.
                                </div>
        `;
        return;
    }
    reviewList.innerHTML = reviews.map(review => {
        const rating = Number(review.rating);

        return `
                <article class="review-item">
                    <div class="review-header">
                        <div class="review-user">
                            <strong>${formatUserId(review.userId)}</strong>
                        </div>
                        <div class="review-rating">
                            ${"★".repeat(rating)}
                            ${"☆".repeat(5 - rating)}
                        </div>
                    </div>
                    <p class="review-content">${review.content}</p>
                    <div class="review-footer">
                        <span>${formatReviewDate(review.createdAt)}</span>
                    </div>
                </article>
            `;
    }).join("");
}

function setReviewSortActive(sort)
{
    const latestButton = document.getElementById("latestSortButton");
    const ratingButton = document.getElementById("ratingSortButton");

    latestButton.classList.toggle("active", sort === "latest");
    ratingButton.classList.toggle("active", sort === "rating");
}

function formatUserId(userId)
{
    if (!userId)
    {
        return "";
    }
    return userId.length > 3 ? `${userId.substring(0, 3)}...` : userId;
}

function formatReviewDate(date)
{
    if (!date)
    {
        return "";
    }
    return new Date(date).toLocaleDateString("ko-KR");
}

async function createReview(movieId)
{
    const rating  = Number(document.getElementById("reviewRating").value);
    const content = document.getElementById("reviewContent").value.trim();

    if (rating < 1 || rating > 5)
    {
        alert("별점은 1점에서 5점 사이여야 합니다.");

        return;
    }

    if (!content)
    {
        alert("리뷰 내용을 입력해주세요.");

        return;
    }

    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Reviews/review`, {
            method: "POST",
            credentials: "include",
            headers: {
                "Content-Type": "application/json",
            },

            body: JSON.stringify({
                movieId: Number(movieId),
                rating: rating,
                content: content
            })
        });

        if (response.status === 401)
        {
            alert("로그인이 필요한 서비스입니다.");

            return;
        }

        const result = await response.json();

        console.log("리뷰 등록 결과 : ", result);

        if (!response.ok)
        {
            throw new Error(result.reMsg || "리뷰 등록에 실패했습니다.");
        }

        if (result.retVal !== 0)
        {
            alert(result.retMsg);

            return;
        }

        alert("리뷰가 등록되었습니다.");

        document.getElementById("reviewRating").value = "5";
        document.getElementById("reviewContent").value = "";
    }
    catch (error)
    {
        console.error("리뷰 등록 오류:", error);

        alert("리뷰 등록 중 오류가 발생했습니다.");
    }
}