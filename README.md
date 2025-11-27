# School Club Site Documentation

## Overview
A web application for viewing and managing school clubs.  Students can browse available clubs, view club details, and join clubs of interest.

## Features

### User Roles
- **Student**: Can view clubs and join/leave clubs
- **Club Admin**: Can manage club information and members
- **Site Admin**: Can manage all clubs and users

### Core Features
- Browse all active clubs
- View club details (description, meeting times, members)
- Join/leave clubs
- Search and filter clubs by category
- View club member lists
- Club contact information

## Project Structure
```
school-club-site/
├── frontend/
│   ├── pages/
│   │   ├── clubs. html
│   │   ├── club-detail.html
│   │   └── dashboard.html
│   ├── styles/
│   │   └── main.css
│   └── scripts/
│       └── app.js
├── backend/
│   ├── routes/
│   │   ├── clubs.js
│   │   └── users.js
│   ├── models/
│   │   ├── Club.js
│   │   └── User.js
│   └── server.js
└── database/
    └── schema.sql
```

## Data Models

### Club
```json
{
  "id": "uuid",
  "name": "string",
  "description": "string",
  "category": "string",
  "meeting_time": "string",
  "location": "string",
  "admin_id": "uuid",
  "members": ["user_id"],
  "created_at": "timestamp"
}
```

### User
```json
{
  "id": "uuid",
  "name": "string",
  "email": "string",
  "role": "student|admin",
  "joined_clubs": ["club_id"],
  "created_at": "timestamp"
}
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/clubs` | Get all clubs |
| GET | `/api/clubs/:id` | Get club details |
| POST | `/api/clubs` | Create new club |
| PUT | `/api/clubs/:id` | Update club |
| DELETE | `/api/clubs/:id` | Delete club |
| POST | `/api/clubs/:id/join` | Join a club |
| POST | `/api/clubs/:id/leave` | Leave a club |
| GET | `/api/users/:id/clubs` | Get user's clubs |

## Getting Started

### Prerequisites
- Node.js 14+
- PostgreSQL or MongoDB
- npm or yarn

### Installation
```bash
# Clone repository
git clone <repo-url>

# Install dependencies
npm install

# Set up environment variables
cp .env.example .env

# Run database migrations
npm run migrate

# Start server
npm start
```

## Usage

1. **View Clubs**: Navigate to `/clubs` to browse all clubs
2. **Club Details**: Click a club card to see full details
3. **Join Club**: Click "Join" button on club page
4. **Manage Club**: Admins can edit club info in settings

## Status
🚧 **Work in Progress**
- [ ] Authentication system
- [ ] Club creation interface
- [ ] Member management dashboard
- [ ] Email notifications
- [ ] Search functionality

## Next Steps
- Set up database schema
- Build API endpoints
- Create frontend components
- Implement authentication
- Add user testing

---
*Last Updated: 2025-11-27*
*Project Status: Early Development*
