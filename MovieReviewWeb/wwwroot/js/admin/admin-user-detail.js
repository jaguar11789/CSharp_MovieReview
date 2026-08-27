// const API_BASE_URL = "https://localhost:7226";
document.addEventListener("DOMContentLoaded", async () => {
    await loadUserDetail();
});

async function loadUserDetail()
{
    const params = new URLSearchParams(location.search); // URL에서 회원 ID 가져오기
    const userId = params.get("id");

    if (!userId)
    {
        alert("회원 정보가 없습니다.");
        location.href = "/admin/users/index";

        return;
    }

    try
    {
        const response = await fetch(`${API_BASE_URL}/api/AdminUser/users/${userId}`,
            {
                credentials: "include"
            });

        if (response.status === 401)
        {
            alert("로그인이 필요한 서비스입니다.");
            location.href = "/accounts/login";

            return;
        }

        // 관리자 권한 없음
        if (response.status === 403)
        {
            alert("관리자 권한이 필요합니다.");
            location.href = "/main";

            return;
        }

        // 회원 없음
        if (response.status === 404)
        {
            alert("존재하지 않는 회원입니다.");
            location.href = "/admin/users/index";

            return;
        }

        if (!response.ok)
        {
            throw new Error(`회원 상세 정보 요청 실패 : ${response.status}`);
        }

        const userInfo = await response.json();
        console.log("회원 상세 정보:", userInfo);

        renderUserDetail(userInfo);
    }
    catch (error)
    {
        console.error("회원 상세 정보 조회 오류:", error);

        alert("회원 정보를 불러오지 못했습니다.");

        location.href = "/admin/users/index";
    }
}

function renderUserDetail(user)
{
    // 회원 요약
    document.getElementById("userAvatar").textContent  = getAvatarText(user.userName, user.userId);
    document.getElementById("userName").textContent    = user.userName || "이름 미등록";
    document.getElementById("userAccount").textContent = user.userId || "-";
    document.getElementById("userEmail").textContent   = user.email || "-";

    const userRole = document.getElementById("userRole");

    userRole.textContent = formatRole(user.role);
    userRole.className   = `user-role ${getRoleClass(user.role)}`;

    const userStatus = document.getElementById("userStatus");

    userStatus.textContent = formatStatus(user.statusCode);
    userStatus.className   = `user-status ${getStatusClass(user.statusCode)}`;

    // 기본 정보
    document.getElementById("detailId").textContent          = user.id ?? "-";
    document.getElementById("detailUserId").textContent      = user.userId || "-";
    document.getElementById("detailUserName").textContent    = user.userName || "-";
    document.getElementById("detailEmail").textContent       = user.email || "-";
    document.getElementById("detailPhoneNumber").textContent = formatPhoneNumber(user.phoneNumber);

    document.getElementById("detailGender").textContent      = formatGender(user.gender);
    document.getElementById("detailBirthDate").textContent   = formatDate(user.birthDate);

    // 주소
    document.getElementById("detailZipCode").textContent       = user.zipCode || "-";
    document.getElementById("detailBaseAddress").textContent   = user.baseAddress || "-";
    document.getElementById("detailDetailAddress").textContent = user.detailAddress || "-";

    // 계정 정보
    const detailRole = document.getElementById("detailRole");

    detailRole.textContent = formatRole(user.role);
    detailRole.className   = `user-role ${getRoleClass(user.role)}`;

    const detailStatus = document.getElementById("detailStatus");

    detailStatus.textContent = formatStatus(user.statusCode);
    detailStatus.className   = `user-status ${getStatusClass(user.statusCode)}`;

    const emailVerified = document.getElementById("detailEmailVerified");

    emailVerified.textContent = formatEmailVerified(user.emailVerified);
    emailVerified.className   = `user-email-verified ${getEmailVerifiedClass(user.emailVerified)}`;

    document.getElementById("detailCreatedAt").textContent = formatDateTime(user.createdAt);
    document.getElementById("detailUpdatedAt").textContent = formatDateTime(user.updatedAt);

    // 활동 이력
    renderUserHistory(user.history);
}

function getRoleClass(role)
{
    switch (role)
    {
        case "Admin":
            return "admin";

        case "User":
            return "user";

        default:
            return "unknown";
    }
}


function formatRole(role)
{
    switch (role)
    {
        case "Admin":
            return "관리자";

        case "User":
            return "사용자";

        default:
            return role || "-";
    }
}


function getStatusClass(statusCode)
{
    switch (statusCode)
    {
        case 100:
            return "normal";

        case 200:
            return "dormant";

        case 300:
            return "suspended";

        case 900:
            return "withdrawn";

        default:
            return "unknown";
    }
}


function formatStatus(statusCode)
{
    switch (statusCode)
    {
        case 100:
            return "정상";

        case 200:
            return "휴면";

        case 990:
            return "정지";

        case 999:
            return "탈퇴 요청";

        default:
            return `상태 ${statusCode ?? "-"}`;
    }
}

function formatDate(date)
{
    if (!date)
    {
        return "-";
    }

    const value = new Date(date);

    if (value.getFullYear() === 1)
    {
        return "-";
    }

    return value.toLocaleDateString("ko-KR");
}

function formatDateTime(date)
{
    if (!date)
    {
        return "-";
    }

    const value = new Date(date);

    if (value.getFullYear() === 1)
    {
        return "-";
    }

    return value.toLocaleString("ko-KR");
}

function formatPhoneNumber(phoneNumber)
{
    if (!phoneNumber)
    {
        return "-";
    }

    if (phoneNumber.length === 11)
    {
        return phoneNumber.replace(
            /(\d{3})(\d{4})(\d{4})/,
            "$1-$2-$3"
        );
    }

    if (phoneNumber.length === 10)
    {
        return phoneNumber.replace(
            /(\d{3})(\d{3})(\d{4})/,
            "$1-$2-$3"
        );
    }

    return phoneNumber;
}

function formatGender(gender)
{
    switch (gender)
    {
        case "M":
            return "남성";

        case "F":
            return "여성";

        default:
            return gender || "-";
    }
}

function getAvatarText(userName, userId)
{
    const value = userName || userId;

    if (!value)
    {
        return "U";
    }

    return value.charAt(0).toUpperCase();
}

function renderUserHistory(history)
{
    const historyList = document.getElementById("userHistory");


    if (!history || history.length === 0)
    {
        historyList.innerHTML = `
                                <tr>
                                    <td colspan="4"
                                        class="admin-table-empty">
                                        회원 활동 이력이 없습니다.
                                    </td>
                                </tr>
                                `;

        return;
    }


    historyList.innerHTML = history.map(item => `
                                                <tr>

                                                    <td>
                                                        ${formatDateTime(item.changedAt)}
                                                    </td>

                                                    <td>
                                                        <span class="history-action">
                                                            ${formatAction(item.actionCode)}
                                                        </span>
                                                    </td>

                                                    <td>
                                                        <span class="user-status ${getStatusClass(item.statusCode)}">
                                                            ${formatStatus(item.statusCode)}
                                                        </span>
                                                    </td>

                                                    <td>
                                                        ${item.memo || "-"}
                                                    </td>

                                                </tr>
                                                `).join("");
}

function formatAction(actionCode)
{
    switch (actionCode)
    {
        case 100:
            return "계정 활성화";

        case 150:
            return "회원가입";

        case 200:
            return "회원 정보 수정";

        case 300:
            return "이메일 인증";

        case 999:
            return "회원 탈퇴 요청";

        case 990:
            return "계정 정지";
        default:
            return `활동 ${actionCode ?? "-"}`;
    }
}

function formatEmailVerified(value)
{
    return value ? "인증 완료" : "미인증";
}


function getEmailVerifiedClass(value)
{
    return value ? "verified" : "unverified";
}