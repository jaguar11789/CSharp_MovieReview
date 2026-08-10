const API_BASE_URL = "https://localhost:7226";

document.addEventListener("DOMContentLoaded", async () => {

    const searchInput  = document.getElementById("movieSearchInput");
    const searchButton = document.getElementById("movieSearchButton");

    searchButton.addEventListener("click", () => {

        const query = searchInput.value.trim();

        if (!query)
        {
            alert("검색어를 입력해주세요.");

            return;
        }
        searchMovies(query);
    });

    searchInput.addEventListener("keydown", (event) => {

        if (event.key === "Enter")
        {
            searchButton.click();
        }
    });

    await loadMovies();
});

function renderMovies(movies)
{
    const movieList = document.getElementById("movieList");

    movieList.innerHTML = movies.map(movie => {

        const posterUrl = movie.poster_path ? `https://image.tmdb.org/t/p/w500${movie.poster_path}` : "/image/no-poster.png";

        return `
                <div
                    class="movie-card" onclick="goToMovie(${movie.id})">
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
async function loadMovies()
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Movies`);

        if (!response.ok)
        {
            throw new Error(`전체 영화 요청 실패 : ${response.status}`);
        }

        const movieData = await response.json();

        console.log(movieData);

        renderMovies(movieData.results);
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

function goToMovie(movieId)
{
    window.location.href = `/movies/detail?id=${movieId}`;
}

async function searchMovies(query)
{
    try
    {
        const response = await fetch(`${API_BASE_URL}/api/Movies/search?query=${encodeURIComponent(query)}&page=1`);

        if (!response.ok) {
            throw new Error(`영화 검색 실패 : ${response.status}`);
        }

        const movieData = await response.json();

        renderMovies(movieData.results);
    }
    catch (error)
    {
        console.error("영화 검색 오류 :", error);
    }
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