const API_BASE_URL = "https://localhost:7226";

const movieGenres = [
    { id: 0,     name: "전체" },
    { id: 28,    name: "액션" },
    { id: 12,    name: "모험" },
    { id: 16,    name: "애니메이션" },
    { id: 35,    name: "코미디" },

    { id: 80,    name: "범죄" },
    { id: 18,    name: "드라마" },
    { id: 14,    name: "판타지" },
    { id: 27,    name: "공포" },
    { id: 9648,  name: "미스터리" },

    { id: 10749, name: "로맨스" },
    { id: 878,   name: "SF" },
    { id: 53,    name: "스릴러" },
    { id: 10752, name: "전쟁" },
    { id: 37,    name: "서부" }
];

// 현재 영화 목록 상태 
let currentMode        = "all";
let currentGenreId     = 0;
let currentSearchQuery = "";

document.addEventListener("DOMContentLoaded", async () => {

    renderGenres();

    const searchInput  = document.getElementById("movieSearchInput");
    const searchButton = document.getElementById("movieSearchButton");

    searchButton.addEventListener("click", async () => {

        const query = searchInput.value.trim();

        if (!query)
        {
            alert("검색어를 입력해주세요.");

            return;
        }
        currentMode        = "search";
        currentSearchQuery = query;

        searchMovies(query, 1);
    });

    searchInput.addEventListener("keydown", event => {

        if (event.key === "Enter")
        {
            searchButton.click();
        }
    });

    await loadMovies();
});

// ==================================================
// 영화 출력
// ==================================================
function renderMovies(movies)
{
    const movieList = document.getElementById("movieList");

    if (!movies?.length)
    {
        movieList.innerHTML = ` <div class="movie-error"> 영화 정보가 없습니다. </div> `;

        return;
    }

    movieList.innerHTML = movies.map(movie => {
        const posterUrl = movie.poster_path ? `https://image.tmdb.org/t/p/w500${movie.poster_path}` : "/image/no-poster.png";

    return `
            <div class="movie-card" onclick="goToMovie(${movie.id})">
                <img src="${posterUrl}" alt="${movie.title}">
                <div class="movie-info">
                    <h3>${movie.title}</h3>
                    <span class="movie-rating">★ ${Number(movie.vote_average).toFixed(1)}</span>
                    <span class="movie-date">${movie.release_date || ""}</span>
                </div>
            </div>
            `;

    }).join("");
}
async function loadMovies(page = 1)
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Movies?page=${page}`);

        if (!response.ok)
        {
            throw new Error(`전체 영화 요청 실패 : ${response.status}`);
        }

        const movieData = await response.json();

        console.log(movieData);

        renderMovies(movieData.results);
        renderPagination(movieData, page => loadMovies(page));
    }
    catch (error)
    {
        console.error("전체 영화 요청 오류 :", error);

        const movieList = document.getElementById("movieList");

        movieList.innerHTML = `
                               <div class="movie-error">
                                   영화 정보를 불러오지 못했습니다.
                               </div>
                              `;
    }
}

// ==================================================
// 장르별 영화
// ==================================================
async function loadMoviesByGenre(genreId, page = 1)
{
    try {
        const response = await fetch(`${API_BASE_URL}/api/Movies/genre/${genreId}?page=${page}`);

        if (!response.ok) {
            throw new Error(`장르별 영화 요청 실패 : ${response.status}`);
        }

        const movieData = await response.json();

        renderMovies(movieData.results);
        renderPagination(movieData, page => loadMoviesByGenre(genreId, page));

    }
    catch (error)
    {
        console.error("장르별 영화 요청 오류 :", error);
    }
}

// ==================================================
// 장르 버튼
// ==================================================
function renderGenres()
{
    const genreContainer = document.getElementById("movieGenres");

    genreContainer.innerHTML = movieGenres.map((genre, index) => {

        return `<button type="button" class="genre-button ${index === 0 ? "active" : ""}" data-genre-id="${genre.id}">${genre.name}</button>`;
    }).join("");

    genreContainer.querySelectorAll(".genre-button").forEach(button => {

        button.addEventListener("click", async () => {

            genreContainer.querySelectorAll(".genre-button").forEach(btn => {
                btn.classList.remove("active");
            });
            button.classList.add("active");

            const genreId = Number(button.dataset.genreId);

            if (genreId === 0)
            {
                currentMode        = "all";
                currentGenreId     = 0;
                currentSearchQuery = "";

                await loadMovies(1);

                return;
            }
            currentMode        = "genre";
            currentGenreId     = genreId;
            currentSearchQuery = "";

            await loadMoviesByGenre(genreId, 1);
        });
    });
}

// ==================================================
// 검색
// ==================================================
async function searchMovies(query, page = 1)
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Movies/search?query=${encodeURIComponent(query)}&page=${page}`);

        if (!response.ok) {
            throw new Error(`영화 검색 실패 : ${response.status}`);
        }

        const movieData = await response.json();

        renderMovies(movieData.results);
        renderPagination(movieData, page => searchMovies(query, page));
    }
    catch (error)
    {
        console.error("영화 검색 오류 :", error);
    }
}

// ==================================================
// 페이징
// ==================================================
function renderPagination(pageData, loadPage)
{
    const pagination = document.getElementById("moviePagination");

    pagination.innerHTML = "";

    const { currentPage, totalPages, startPage, endPage } = pageData;

    if (totalPages <= 1)
    {
        return;
    }

    // 이전 버튼
    if (startPage > 1)
    {

        const prevButton = document.createElement("button");

        prevButton.type        = "button";
        prevButton.className   = "page-button";
        prevButton.textContent = "‹";

        prevButton.addEventListener("click", () => {
            loadPage(startPage - 1);
        });

        pagination.appendChild(prevButton);
    }

    // 페이지 번호
    for (let page = startPage; page <= endPage; page++)
    {
        const pageButton = document.createElement("button");

        pageButton.type      = "button";
        pageButton.className = `page-button ${page === currentPage ? "active" : ""}`;

        pageButton.textContent = page;

        pageButton.addEventListener("click", () => {
            loadPage(page);
        });

        pagination.appendChild(pageButton);
    }

    // 다음 버튼
    if (endPage < totalPages) {
        const nextButton = document.createElement("button");

        nextButton.type        = "button";
        nextButton.className   = "page-button";
        nextButton.textContent = "›";

        nextButton.addEventListener("click", () => {
            loadPage(endPage + 1);
        });

        pagination.appendChild(nextButton);
    }
}

function goToMovie(movieId)
{
    window.location.href = `/movies/detail?id=${movieId}`;
}



/*
async function loadMovies()
{

const movieList = document.getElementById("movieList");

try
{
    const response = await fetch(`${API_BASE_URL}/api/Movies`);

    if (!response.ok)
    {
        throw new Error(`전체 영화 요청 실패 : ${response.status}`);
    }

    const movieData = await response.json();

    console.log(movieData);

    movieList.innerHTML = movieData.results.map(movie => {

        const posterUrl = movie.poster_path ? `https://image.tmdb.org/t/p/w500${movie.poster_path}` : "/image/no-poster.png";

        return `
                <div class="movie-card" onclick="goToMovie(${movie.id})">
                    <img src="${posterUrl}" alt="${movie.title}">
                    <div class="movie-info">
                        <h3>${movie.title}</h3>
                        <span class="movie-rating">★ ${Number(movie.vote_average).toFixed(1)}</span>
                        <span class="movie-date">${movie.release_date || ""}</span>
                    </div>
                </div>
                `;
    }).join("");
}
catch (error)
{
    console.error("전체 영화 요청 오류 :", error);

    movieList.innerHTML = `
                            <div class="movie-error">
                                영화 정보를 불러오지 못했습니다.
                            </div>
                          `;
}
}
*/